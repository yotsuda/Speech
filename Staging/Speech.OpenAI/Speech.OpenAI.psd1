@{
    RootModule = 'Speech.OpenAI.dll'
    ModuleVersion = '0.3.0'
    GUID = 'e5f6a7b8-c9d0-4e5f-2a3b-4c5d6e7f8a9b'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2026 Yoshifumi Tsuda. MIT License.'
    Description = 'OpenAI Speech module - TTS using OpenAI API and STT using Whisper'
    PowerShellVersion = '7.0'

    RequiredModules = @(
        @{ModuleName='Speech.Core'; ModuleVersion='0.3.0'}
    )

    FormatsToProcess = @('Speech.OpenAI.format.ps1xml')

    CmdletsToExport = @(
        'Out-OpenAISpeech',
        'Get-OpenAISpeech',
        'Read-OpenAISpeech',
        'Set-OpenAISpeechConfig'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'OpenAI', 'Whisper', 'GPT')
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. TTS using OpenAI API and STT using Whisper.'
        }
    }
}
