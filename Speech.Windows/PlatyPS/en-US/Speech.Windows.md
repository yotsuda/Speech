---
Module Name: Speech.Windows
Module Guid: c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f
Download Help Link: https://github.com/yotsuda/Speech
Help Version: 0.3.0
Locale: en-US
---

# Speech.Windows Module
## Description
Windows Speech API (SAPI) module. TTS and STT using built-in Windows speech engine. No API key, no internet required.

## Prerequisites

### Voices
Windows includes default TTS voices. To add more:

**Via Windows Settings:**
Settings > Time & language > Speech > Manage voices > Add voices

**Via PowerShell:**
```powershell
Get-WindowsSpeech                    # List installed voices
Get-WindowsSpeech -Culture ja-JP     # Filter by culture
```

### Speech Recognition Language Packs
For non-English STT, install language packs:

Settings > Time & language > Language & region > Add language > Download "Speech" feature

## Quick Start
```powershell
Out-WindowsSpeech "Hello, world!"             # No setup needed!
Get-WindowsSpeech                              # List voices
Set-WindowsSpeechConfig -Voice "Microsoft Haruka Desktop"  # Set voice
$text = Read-WindowsSpeech -Language ja-JP     # STT
```

## Speech.Windows Cmdlets
### [Get-WindowsSpeech](Get-WindowsSpeech.md)
List installed Windows SAPI voices.

### [Out-WindowsSpeech](Out-WindowsSpeech.md)
Speak text aloud using Windows SAPI (TTS).

### [Read-WindowsSpeech](Read-WindowsSpeech.md)
Speech recognition using Windows SAPI (STT).

### [Set-WindowsSpeechConfig](Set-WindowsSpeechConfig.md)
Configure Windows-specific speech settings.