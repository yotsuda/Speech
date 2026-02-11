@{
    RootModule = 'Speech.OpenAI.dll'
    ModuleVersion = '0.3.0'
    GUID = 'e5f6a7b8-c9d0-4e5f-2a3b-4c5d6e7f8a9b'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2025 Yoshifumi Tsuda. MIT License.'
    Description = 'OpenAI Speech module - TTS using OpenAI API and STT using Whisper'
    PowerShellVersion = '7.0'

    RequiredModules = @('Speech.Core')

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
            ProjectUri = 'https://github.com/yotsuda/Speech'
        }
    }
}
