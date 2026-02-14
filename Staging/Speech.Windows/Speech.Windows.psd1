@{
    RootModule = 'Speech.Windows.dll'
    ModuleVersion = '0.3.0'
    GUID = 'c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f'
    CompatiblePSEditions = @('Core')
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2026 Yoshifumi Tsuda. MIT License.'
    Description = 'Windows Speech API module for Speech - TTS and STT using System.Speech'
    PowerShellVersion = '7.4'

    RequiredModules = @(
        @{ModuleName='Speech.Core'; ModuleVersion='0.3.0'}
    )

    FormatsToProcess = @('Speech.Windows.format.ps1xml')

    CmdletsToExport = @(
        'Out-WindowsSpeech',
        'Get-WindowsSpeech',
        'Read-WindowsSpeech',
        'Set-WindowsSpeechConfig'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Windows', 'SAPI')
            IconUri = 'https://raw.githubusercontent.com/yotsuda/Speech/main/assets/icon.svg'
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. TTS and STT using Windows Speech API (System.Speech).'
        }
    }
}
