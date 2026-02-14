@{
    RootModule = 'Speech.Core.dll'
    ModuleVersion = '0.3.0'
    GUID = 'b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e'
    CompatiblePSEditions = @('Core')
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2026 Yoshifumi Tsuda. MIT License.'
    Description = 'Core module for Speech - configuration management and utilities'
    PowerShellVersion = '7.0'

    RequiredAssemblies = @(
        'NAudio.dll',
        'NAudio.Core.dll',
        'NAudio.Wasapi.dll',
        'NAudio.WinMM.dll'
    )

    CmdletsToExport = @(
        'Get-SpeechConfig',
        'Set-SpeechConfig',
        'Get-Microphone',
        'Test-Microphone'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'Configuration')
            IconUri = 'https://raw.githubusercontent.com/yotsuda/Speech/main/assets/icon.svg'
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. Core configuration management and audio utilities for the Speech module family.'
        }
    }
}
