@{
    ModuleVersion = '0.3.0'
    GUID = 'a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d'
    Author = 'Yoshifumi Tsuda'
    Copyright = 'Copyright (c) 2026 Yoshifumi Tsuda. MIT License.'
    Description = 'PowerShell module for speech synthesis and recognition - includes Windows, Azure, OpenAI, and Google speech services'
    PowerShellVersion = '7.0'

    RequiredModules = @(
        @{ModuleName='Speech.Core'; ModuleVersion='0.3.0'}
        @{ModuleName='Speech.Windows'; ModuleVersion='0.3.0'}
        @{ModuleName='Speech.Azure'; ModuleVersion='0.3.0'}
        @{ModuleName='Speech.OpenAI'; ModuleVersion='0.3.0'}
        @{ModuleName='Speech.Google'; ModuleVersion='0.3.0'}
    )

    CmdletsToExport = @()
    FunctionsToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Speech', 'TTS', 'STT', 'Recognition', 'Azure', 'Windows', 'OpenAI', 'Whisper', 'Google', 'Cloud')
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. Unified speech module including Windows, Azure, OpenAI, and Google speech services.'
        }
    }
}
