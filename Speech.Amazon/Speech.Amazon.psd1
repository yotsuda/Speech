@{
    RootModule = 'Speech.Amazon.dll'
    ModuleVersion = '0.3.0'
    GUID = '497d80c4-1310-4386-9b8e-6f843d845ce6'
    CompatiblePSEditions = @('Core')
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2026 Yoshifumi Tsuda. MIT License.'
    Description = 'Amazon Polly and Transcribe for Speech module - Text-to-Speech and Speech-to-Text'
    PowerShellVersion = '7.4'

    RequiredModules = @(
        @{ModuleName='Speech.Core'; ModuleVersion='0.3.0'}
    )

    FormatsToProcess = @('Speech.Amazon.format.ps1xml')

    CmdletsToExport = @(
        'Out-AmazonSpeech',
        'Read-AmazonSpeech',
        'Get-AmazonSpeech',
        'Set-AmazonSpeechConfig'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Amazon', 'Polly', 'Transcribe', 'AWS')
            IconUri = 'https://raw.githubusercontent.com/yotsuda/Speech/main/assets/icon.svg'
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. TTS using Amazon Polly and STT using Amazon Transcribe Streaming.'
        }
    }
}
