@{
    ModuleVersion = '0.3.0'
    GUID = '144e10f2-4dae-4e60-95a4-fc5b5247e3de'
    CompatiblePSEditions = @('Core')
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
            IconUri = 'https://raw.githubusercontent.com/yotsuda/Speech/main/assets/icon.svg'
            LicenseUri = 'https://github.com/yotsuda/Speech/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yotsuda/Speech'
            ReleaseNotes = 'v0.3.0 - Initial release. Unified speech module including Windows, Azure, OpenAI, and Google speech services.'
        }
    }
}
