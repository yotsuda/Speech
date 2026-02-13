@{
    RootModule = 'Speech.Azure.dll'
    ModuleVersion = '0.3.0'
    GUID = 'd4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2026 Yoshifumi Tsuda. MIT License.'
    Description = 'Azure Speech Services module for Speech - TTS and STT using Azure Cognitive Services'
    PowerShellVersion = '7.0'

    RequiredModules = @(
        @{ModuleName='Speech.Core'; ModuleVersion='0.3.0'}
    )

    FormatsToProcess = @('Speech.Azure.format.ps1xml')

    CmdletsToExport = @(
        'Out-AzureSpeech',
        'Get-AzureSpeech',
        'Read-AzureSpeech',
        'Set-AzureSpeechConfig'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Azure', 'CognitiveServices')
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. TTS and STT using Azure Cognitive Services Speech SDK.'
        }
    }
}
