@{
    ModuleVersion = '0.3.0'
    GUID = 'a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2025 Yoshifumi Tsuda. MIT License.'
    Description = 'PowerShell module for speech synthesis and recognition - includes Windows, Azure, and OpenAI speech services'
    PowerShellVersion = '7.0'

    RequiredModules = @(
        'Speech.Core',
        'Speech.Windows',
        'Speech.Azure',
        'Speech.OpenAI'
    )

    CmdletsToExport = @()
    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Recognition', 'Azure', 'Windows', 'OpenAI', 'Whisper')
            ProjectUri = 'https://github.com/yotsuda/Speech'
        }
    }
}
