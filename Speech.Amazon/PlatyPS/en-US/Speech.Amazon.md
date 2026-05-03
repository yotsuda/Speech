---
Module Name: Speech.Amazon
Module Guid: 497d80c4-1310-4386-9b8e-6f843d845ce6
Download Help Link: https://github.com/yotsuda/Speech
Help Version: 0.3.0
Locale: en-US
---

# Speech.Amazon Module
## Description
Amazon Polly (TTS) and Amazon Transcribe Streaming (STT). Polly offers Standard, Neural, Long-Form, and Generative voices across 30+ languages.

## Prerequisites

### 1. Create AWS account and IAM user
**Via Web:**
1. Go to https://console.aws.amazon.com and sign in (or sign up).
2. IAM > Users > Create user > attach policies `AmazonPollyReadOnlyAccess` and `AmazonTranscribeFullAccess`.
3. Create access key (Use case: "Application running outside AWS"). Save Access Key ID + Secret Access Key.

**Via AWS CLI (PowerShell):**
```powershell
winget install Amazon.AWSCLI
aws configure  # paste keys + region (e.g., us-east-1)
```

**Pricing (free tier, first 12 months):** Polly Standard 5M chars/month, Neural 1M chars/month. Transcribe 60 min/month.

### 2. Configure
```powershell
Set-AmazonSpeechConfig -AccessKey "AKIA..." -SecretKey "..." -Region "us-east-1"
Get-AmazonSpeech -Language ja-JP | Format-Table Name, Gender, Engines
Set-AmazonSpeechConfig -Voice "Mizuki"
Get-SpeechConfig
```

### 3. Test
```powershell
Out-AmazonSpeech "Hello, world!"
$text = Read-AmazonSpeech -Language ja-JP
```

## Speech.Amazon Cmdlets
### [Get-AmazonSpeech](Get-AmazonSpeech.md)
List available Amazon Polly voices.

### [Out-AmazonSpeech](Out-AmazonSpeech.md)
Speak text aloud using Amazon Polly.

### [Read-AmazonSpeech](Read-AmazonSpeech.md)
Speech recognition using Amazon Transcribe Streaming.

### [Set-AmazonSpeechConfig](Set-AmazonSpeechConfig.md)
Configure Amazon-specific speech settings.
