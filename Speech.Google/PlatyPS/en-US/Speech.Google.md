---
Module Name: Speech.Google
Module Guid: c3d4e5f6-a7b8-4c5d-9e0f-1a2b3c4d5e6f
Download Help Link: https://github.com/yotsuda/Speech
Help Version: 0.3.0
Locale: en-US
---

# Speech.Google Module
## Description
Google Cloud Speech module. TTS and STT using Google Cloud APIs. WaveNet, Neural2, and Standard voices across 40+ languages.

## Prerequisites

### 1. Create Google Cloud Project
**Via Web:**
1. Go to https://console.cloud.google.com
2. Create project, enable "Cloud Text-to-Speech API" and "Cloud Speech-to-Text API"
3. Create service account key (JSON) under IAM > Service Accounts

**Via gcloud CLI (PowerShell):**
```powershell
winget install Google.CloudSDK
gcloud auth login
gcloud projects create my-speech --set-as-default
gcloud services enable texttospeech.googleapis.com speech.googleapis.com
gcloud iam service-accounts create speech-sa
gcloud iam service-accounts keys create credential.json --iam-account speech-sa@my-speech.iam.gserviceaccount.com
```

**Pricing:** Standard TTS free 4M chars/month, WaveNet/Neural2 free 1M chars/month, STT free 60 min/month.

### 2. Configure
```powershell
Set-GoogleSpeechConfig -Credential "C:\path\to\credential.json"
Get-GoogleSpeech -Language ja-JP | Format-Table Name, Gender
Set-GoogleSpeechConfig -Voice "ja-JP-Neural2-B"
Get-SpeechConfig
```

### 3. Test
```powershell
Out-GoogleSpeech "Hello, world!"
$text = Read-GoogleSpeech -Language ja-JP
```

## Speech.Google Cmdlets
### [Get-GoogleSpeech](Get-GoogleSpeech.md)
List available Google Cloud TTS voices.

### [Out-GoogleSpeech](Out-GoogleSpeech.md)
Speak text aloud using Google Cloud TTS.

### [Read-GoogleSpeech](Read-GoogleSpeech.md)
Speech recognition using Google Cloud STT.

### [Set-GoogleSpeechConfig](Set-GoogleSpeechConfig.md)
Configure Google Cloud-specific speech settings.