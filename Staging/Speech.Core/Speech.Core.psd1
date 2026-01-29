@{
    RootModule = 'Speech.Core.dll'
    ModuleVersion = '0.3.0'
    GUID = 'b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2025 Yoshifumi Tsuda. MIT License.'
    Description = 'Core module for Speech - configuration management and utilities'
    PowerShellVersion = '7.0'

    CmdletsToExport = @(
        'Get-SpeechConfig',
        'Set-SpeechConfig',
        'Get-Microphone'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'Configuration')
            ProjectUri = 'https://github.com/yotsuda/Speech'
        }
    }
}
