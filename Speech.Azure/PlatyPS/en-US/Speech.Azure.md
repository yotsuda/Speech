---
Module Name: Speech.Azure
Module Guid: d4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a
Download Help Link: https://github.com/yotsuda/Speech
Help Version: 0.3.0
Locale: en-US
---

# Speech.Azure Module
## Description
Azure Cognitive Services Speech module. High-quality TTS and real-time STT using Azure Speech Services. 400+ neural voices across 140+ languages.

## Prerequisites

### 1. Create Azure Speech Resource

**Via Azure Portal (Web):**
1. Go to https://portal.azure.com
2. Create a resource > "Speech" > Create
3. Select pricing tier (Free F0: 0.5M chars TTS + 5h STT per month)
4. Copy **Key** and **Region** from "Keys and Endpoint" page

**Via Azure CLI (PowerShell):**
```powershell
winget install Microsoft.AzureCLI
az login
az cognitiveservices account create -n my-speech -g my-rg --kind SpeechServices --sku F0 -l eastus
az cognitiveservices account keys list -n my-speech -g my-rg --query key1 -o tsv
```

### 2. Configure
```powershell
Set-AzureSpeechConfig -Key "your-key" -Region "eastus"
Get-AzureSpeech -Locale ja | Format-Table ShortName, DisplayName, Gender
Set-AzureSpeechConfig -Voice "ja-JP-NanamiNeural"
Get-SpeechConfig
```

### 3. Test
```powershell
Out-AzureSpeech "Hello, world!"
$text = Read-AzureSpeech -Language en-US
```

## Speech.Azure Cmdlets
### [Get-AzureSpeech](Get-AzureSpeech.md)
List available Azure neural voices.

### [Out-AzureSpeech](Out-AzureSpeech.md)
Speak text aloud using Azure Speech Services (TTS).

### [Read-AzureSpeech](Read-AzureSpeech.md)
Real-time speech recognition using Azure Speech Services (STT).

### [Set-AzureSpeechConfig](Set-AzureSpeechConfig.md)
Configure Azure-specific speech settings.