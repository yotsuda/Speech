---
Module Name: Speech.OpenAI
Module Guid: e5f6a7b8-c9d0-4e5f-2a3b-4c5d6e7f8a9b
Download Help Link: https://github.com/yotsuda/Speech
Help Version: 0.3.0
Locale: en-US
---

# Speech.OpenAI Module
## Description
OpenAI Speech module. TTS via OpenAI Audio API and STT via Whisper. Supports multiple voices and models.

## Prerequisites

### 1. Get an OpenAI API Key
**Via Web:**
1. Go to https://platform.openai.com/api-keys
2. Create new secret key, copy it (shown only once)
3. Add billing: https://platform.openai.com/settings/organization/billing

**Pricing:** TTS ~$15/1M chars (tts-1), STT ~$0.006/min (whisper-1)

### 2. Configure
```powershell
Set-OpenAISpeechConfig -Key "sk-..."
Get-OpenAISpeech    # list voices
Set-OpenAISpeechConfig -Voice "nova" -Model "tts-1"
Get-SpeechConfig
```

### 3. Test
```powershell
Out-OpenAISpeech "Hello, world!"
$text = Read-OpenAISpeech -Language en
```

## Speech.OpenAI Cmdlets
### [Get-OpenAISpeech](Get-OpenAISpeech.md)
List available OpenAI TTS voices.

### [Out-OpenAISpeech](Out-OpenAISpeech.md)
Speak text aloud using OpenAI TTS API.

### [Read-OpenAISpeech](Read-OpenAISpeech.md)
Speech recognition using OpenAI Whisper (STT).

### [Set-OpenAISpeechConfig](Set-OpenAISpeechConfig.md)
Configure OpenAI-specific speech settings.