---
Module Name: Speech.Core
Module Guid: b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e
Download Help Link: https://github.com/yotsuda/Speech
Help Version: 0.3.0
Locale: en-US
---

# Speech.Core Module
## Description
Core module for the Speech module family. Provides shared configuration management, microphone utilities, and common settings used by all provider modules (Speech.Azure, Speech.OpenAI, Speech.Google, Speech.Windows).

This module is automatically installed as a dependency when you install any provider module.

## Quick Start

```powershell
# List available microphones
Get-Microphone

# Test microphone input
Test-Microphone

# Set preferred microphone for all speech commands
Set-SpeechConfig -Microphone "Headset Microphone"

# Set default language
Set-SpeechConfig -Language ja-JP

# View current configuration
Get-SpeechConfig
```

## Configuration
Settings are stored in `~/.speech/config.json`. Common settings (Rate, Volume, Language, Microphone, OutputDevice) apply across all providers. Provider-specific settings are managed by each provider module:

- `Set-AzureSpeechConfig` - Azure voice, key, region, pitch
- `Set-OpenAISpeechConfig` - OpenAI voice, model, STT model, key
- `Set-GoogleSpeechConfig` - Google voice, credential path
- `Set-WindowsSpeechConfig` - Windows SAPI voice

## Speech.Core Cmdlets
### [Get-Microphone](Get-Microphone.md)
List available microphone input devices.

### [Get-SpeechConfig](Get-SpeechConfig.md)
Display the current speech configuration.

### [Set-SpeechConfig](Set-SpeechConfig.md)
Set common speech configuration settings shared across all providers.

### [Test-Microphone](Test-Microphone.md)
Test microphone input levels with a visual meter.