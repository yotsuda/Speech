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

Describe 'Get-SpeechConfig' {
    It 'returns a string (formatted config display)' {
        $result = Get-SpeechConfig
        $result | Should -BeOfType [string]
    }

    It 'returns a file path string when -Path is specified' {
        $result = Get-SpeechConfig -Path
        $result | Should -BeOfType [string]
        $result | Should -Match 'SpeechConfig\.json$'
    }

    It 'path should contain the Speech module directory' {
        $result = Get-SpeechConfig -Path
        $result | Should -Match 'Speech'
    }
}

Describe 'Set-SpeechConfig' {
    BeforeAll {
        # Save original config path for cleanup
        $script:configPath = Get-SpeechConfig -Path
        $script:backupPath = "$($script:configPath).test_backup"

        # Backup existing config if present
        if (Test-Path $script:configPath) {
            Copy-Item $script:configPath $script:backupPath -Force
        }
    }

    AfterAll {
        # Restore original config
        if (Test-Path $script:backupPath) {
            Copy-Item $script:backupPath $script:configPath -Force
            Remove-Item $script:backupPath -Force
        }
    }

    It 'saves Rate setting successfully' {
        Set-SpeechConfig -Rate 1.5
        $config = Get-SpeechConfig
        $config | Should -Match '1\.50'
    }

    It 'saves Volume setting successfully' {
        Set-SpeechConfig -Volume 80
        $config = Get-SpeechConfig
        $config | Should -Match '80'
    }

    It 'rejects Rate above ValidateRange maximum with an error' {
        { Set-SpeechConfig -Rate 3.0 } | Should -Throw
    }

    It 'rejects Rate below ValidateRange minimum with an error' {
        { Set-SpeechConfig -Rate 0.1 } | Should -Throw
    }

    It 'rejects Volume above ValidateRange maximum with an error' {
        { Set-SpeechConfig -Volume 200 } | Should -Throw
    }
}

Describe 'Get-Microphone' {
    It 'executes without throwing an error' -Skip:(-not (Get-Command Get-Microphone -ErrorAction SilentlyContinue) -or
        -not ([System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.GetName().Name -eq 'NAudio.WinMM' })) {
        { Get-Microphone } | Should -Not -Throw
    }
}
