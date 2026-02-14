BeforeAll {
    if (-not (Get-Module Speech.Core)) {
        $modulePath = Join-Path $PSScriptRoot '..' 'Speech.Core' 'bin' 'Debug' 'net9.0' 'Speech.Core.dll'
        if (-not (Test-Path $modulePath)) {
            $modulePath = Join-Path $PSScriptRoot '..' 'Speech.Core' 'bin' 'Release' 'net9.0' 'Speech.Core.dll'
        }
        if (Test-Path $modulePath) {
            Import-Module $modulePath
        } else {
            throw "Speech.Core.dll not found. Run 'dotnet build' first."
        }
    }
}

Describe 'Configuration File I/O' {
    BeforeAll {
        $script:configPath = Get-SpeechConfig -Path
        $script:backupPath = "$($script:configPath).test_backup"

        # Backup existing config
        if (Test-Path $script:configPath) {
            Copy-Item $script:configPath $script:backupPath -Force
        }
    }

    AfterAll {
        # Restore original config
        if (Test-Path $script:backupPath) {
            Copy-Item $script:backupPath $script:configPath -Force
            Remove-Item $script:backupPath -Force
        } elseif (Test-Path $script:configPath) {
            # If no backup existed, the file was created by tests - leave it
        }
    }

    Context 'Default config creation' {
        It 'creates a config file when none exists' {
            # Remove config if it exists
            if (Test-Path $script:configPath) {
                Remove-Item $script:configPath -Force
            }

            # Get-SpeechConfig warns when file is missing (by design)
            # Set-SpeechConfig triggers ConfigManager.GetConfig() which auto-creates
            Set-SpeechConfig -Rate 1.0

            # Config file should now exist
            Test-Path $script:configPath | Should -Be $true
        }
    }

    Context 'Config persistence (Set → Get roundtrip)' {
        It 'persists Rate across Set and Get' {
            Set-SpeechConfig -Rate 1.2
            $config = Get-SpeechConfig
            $config | Should -Match '1\.20'
        }

        It 'persists Volume across Set and Get' {
            Set-SpeechConfig -Volume 60
            $config = Get-SpeechConfig
            $config | Should -Match '60'
        }

        It 'persists Language across Set and Get' {
            Set-SpeechConfig -Language 'ja-JP'
            $config = Get-SpeechConfig
            $config | Should -Match 'ja-JP'
        }
    }

    Context 'Invalid JSON resilience' {
        It 'recovers gracefully from corrupted config file' {
            # Write invalid JSON
            '{ invalid json content' | Set-Content $script:configPath -Force

            # Get-SpeechConfig should handle the error and return something useful
            # (either warning + recreated config, or the default display)
            { Get-SpeechConfig } | Should -Not -Throw
        }
    }
}
