@{
    RootModule = 'Speech.dll'
    NestedModules = @('Speech.psm1')
    FormatsToProcess = @('Speech.Format.ps1xml')
    ModuleVersion = '0.3.0'
    GUID = 'a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2025 Yoshifumi Tsuda. MIT License.'
    Description = 'PowerShell module for speech synthesis and recognition across Windows Speech API and Azure Speech Services'
    PowerShellVersion = '7.0'

    CmdletsToExport = @(
        # Windows Speech API
        'Out-WindowsSpeech',
        'Get-WindowsSpeech',
        'Read-WindowsSpeech',

        # Azure Speech Services
        'Out-AzureSpeech',
        'Get-AzureSpeech',
        'Read-AzureSpeech',

        # Utilities
        'Get-SpeechConfig',
        'Set-SpeechConfig',
        'Get-SpeechQueueState',
        'Clear-SpeechQueue',
        'Test-Microphone',
        'Get-Microphone'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Recognition', 'Azure', 'Windows')
            ProjectUri = 'https://github.com/yotsuda/Speech'
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/master/LICENSE'
        }
    }
}
