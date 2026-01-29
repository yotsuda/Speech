@{
    RootModule = 'Speech.Windows.dll'
    ModuleVersion = '0.3.0'
    GUID = 'c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2025 Yoshifumi Tsuda. MIT License.'
    Description = 'Windows Speech API module for Speech - TTS and STT using System.Speech'
    PowerShellVersion = '7.0'

    RequiredModules = @('Speech.Core')

    CmdletsToExport = @(
        'Out-WindowsSpeech',
        'Get-WindowsSpeech',
        'Read-WindowsSpeech',
        'Test-Microphone'
    )

    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Windows', 'SAPI')
            ProjectUri = 'https://github.com/yotsuda/Speech'
        }
    }
}
