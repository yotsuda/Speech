@{
    RootModule = 'Speech.Google.dll'
    ModuleVersion = '0.3.0'
    GUID = 'c3d4e5f6-a7b8-4c5d-9e0f-1a2b3c4d5e6f'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2026 Yoshifumi Tsuda. MIT License.'
    Description = 'Google Cloud Speech Services for Speech module - Text-to-Speech and Speech-to-Text'
    PowerShellVersion = '7.0'

    RequiredModules = @(
        @{ModuleName='Speech.Core'; ModuleVersion='0.3.0'}
    )

    FormatsToProcess = @('Speech.Google.format.ps1xml')

    CmdletsToExport = @(
        'Out-GoogleSpeech',
        'Read-GoogleSpeech',
        'Get-GoogleSpeech',
        'Set-GoogleSpeechConfig'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Google', 'Cloud')
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. TTS and STT using Google Cloud Speech Services.'
        }
    }
}
