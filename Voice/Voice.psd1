@{
    RootModule = 'Voice.dll'
    NestedModules = @('Voice.psm1')
    ModuleVersion = '0.3.0'
    GUID = 'a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2025 Yoshifumi Tsuda. MIT License.'
    Description = 'PowerShell module for voice synthesis and recognition across Windows Speech API and Azure Speech Services'
    PowerShellVersion = '7.0'

    CmdletsToExport = @(
        # Windows Speech API
        'Out-WindowsVoice',
        'Get-WindowsVoice',
        'Read-WindowsVoice',

        # Azure Speech Services
        'Out-AzureVoice',
        'Get-AzureVoice',
        'Read-AzureVoice',

        # Utilities
        'Get-VoiceConfig',
        'Set-VoiceConfig',
        'Get-VoiceQueueState',
        'Clear-VoiceQueue',
        'Test-Microphone'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'Voice', 'TTS', 'STT', 'Recognition', 'Azure', 'Windows')
            ProjectUri = 'https://github.com/yotsuda/Voice'
            LicenseUri = 'https://github.com/yotsuda/Voice/blob/master/LICENSE'
        }
    }
}
