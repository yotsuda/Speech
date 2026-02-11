# Speech

PowerShell modules for text-to-speech (TTS) and speech-to-text (STT) across multiple providers.

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-blue)]() [![PowerShell 7.0+](https://img.shields.io/badge/PowerShell-7.0+-blue)]() [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## Providers

| Module | TTS | STT | Requires |
|--------|-----|-----|----------|
| **Speech.Windows** | Offline SAPI | Offline SAPI | Windows 10/11 |
| **Speech.Azure** | 400+ neural voices | Real-time streaming | Azure Speech key |
| **Speech.OpenAI** | 11 multilingual voices | Whisper (batch) | OpenAI API key |
| **Speech.Google** | Standard/WaveNet/Neural2 | Batch | Google Cloud credential JSON |
| **Speech.Core** | — | — | (shared config, microphone, output device) |

## Quick Start

```powershell
# Windows — no setup needed
Out-WindowsSpeech "Hello, world!"

# Azure
Set-AzureSpeechConfig -Key "your-key" -Region "eastus"
Out-AzureSpeech "Hello" -Language en-US

# OpenAI
Set-OpenAISpeechConfig -Key "sk-..."
Out-OpenAISpeech "Hello" -Voice nova

# Google
Set-GoogleSpeechConfig -Credential "path/to/key.json"
Out-GoogleSpeech "Hello"

# Speech recognition (all providers)
$text = Read-WindowsSpeech
$text = Read-AzureSpeech -Language ja-JP
$text = Read-OpenAISpeech -Language ja
$text = Read-GoogleSpeech -Language ja-JP
```

## Installation

### Build from Source

```powershell
git clone https://github.com/yotsuda/Speech.git
cd Speech
dotnet build Speech.sln -c Release
```

### Deploy

```powershell
# Requires elevated PowerShell
.\Deploy.ps1
```

This publishes all 5 modules to `C:\Program Files\PowerShell\7\Modules` with help files.

## Cmdlet Reference

Each provider has 4 cmdlets following a consistent pattern:

| Verb | Purpose | Example |
|------|---------|---------|
| `Out-*Speech` | Text-to-speech | `Out-AzureSpeech "Hello"` |
| `Read-*Speech` | Speech-to-text | `$text = Read-AzureSpeech` |
| `Get-*Speech` | List voices | `Get-AzureSpeech -Locale ja` |
| `Set-*SpeechConfig` | Configure provider | `Set-AzureSpeechConfig -Voice "..."` |

Plus shared cmdlets in Speech.Core: `Get-SpeechConfig`, `Set-SpeechConfig`, `Get-Microphone`, `Test-Microphone`.

Use `Get-Help <cmdlet> -Full` for detailed documentation.

<details>
<summary>All 20 cmdlets</summary>

**Speech.Core** — Shared configuration and audio devices
- `Get-SpeechConfig` — Display current configuration (`-Path` for file location)
- `Set-SpeechConfig` — Set common settings: `-Rate`, `-Volume`, `-Language`, `-Microphone`, `-OutputDevice`
- `Get-Microphone` — List audio input devices
- `Test-Microphone` — Test microphone input level

**Speech.Azure** — Azure Cognitive Services
- `Out-AzureSpeech` — TTS with SSML prosody (`-Rate`, `-Volume`, `-Pitch`, `-Language`, `-Voice`)
- `Read-AzureSpeech` — Real-time streaming STT (`-Language`, `-Detailed`)
- `Get-AzureSpeech` — List 400+ neural voices (`-Locale` to filter)
- `Set-AzureSpeechConfig` — Set `-Key`, `-Region`, `-Voice`, `-Pitch`

**Speech.OpenAI** — OpenAI Audio API
- `Out-OpenAISpeech` — TTS with 11 voices (`-Voice`, `-Model`, `-Speed`)
- `Read-OpenAISpeech` — Whisper batch STT (`-Language`, `-Model`)
- `Get-OpenAISpeech` — List available voices
- `Set-OpenAISpeechConfig` — Set `-Key`, `-Voice`, `-Model`, `-STTModel`

**Speech.Google** — Google Cloud Speech
- `Out-GoogleSpeech` — TTS with Standard/WaveNet/Neural2 (`-Voice`, `-Language`, `-Speed`)
- `Read-GoogleSpeech` — Batch STT (`-Language`)
- `Get-GoogleSpeech` — List available voices (`-Language` to filter)
- `Set-GoogleSpeechConfig` — Set `-Voice`, `-Credential`

**Speech.Windows** — Windows SAPI
- `Out-WindowsSpeech` — Offline TTS (`-Voice`, `-Rate`, `-Volume`)
- `Read-WindowsSpeech` — Offline STT (`-Language`, `-Confidence`, `-Detailed`)
- `Get-WindowsSpeech` — List installed SAPI voices (`-Culture` to filter)
- `Set-WindowsSpeechConfig` — Set `-Voice`

</details>

## Configuration

Settings are stored in `~/.speech/config.json`. API keys are masked when displayed.

```powershell
Get-SpeechConfig          # View all settings
Get-SpeechConfig -Path    # Get config file path
```

<details>
<summary>Provider setup</summary>

### Azure Speech Services

```powershell
# Get key: Azure Portal > Create "Speech" resource > Keys and Endpoint
# Free tier (F0): 0.5M chars TTS + 5h STT / month
Set-AzureSpeechConfig -Key "your-key" -Region "eastus"
Get-AzureSpeech -Locale ja | Format-Table ShortName, DisplayName, Gender
Set-AzureSpeechConfig -Voice "ja-JP-NanamiNeural"
```

### OpenAI

```powershell
# Get key: https://platform.openai.com/api-keys
Set-OpenAISpeechConfig -Key "sk-..."
Set-OpenAISpeechConfig -Voice nova -Model tts-1
```

### Google Cloud

```powershell
# Get credential: Google Cloud Console > IAM > Service Accounts > Create key (JSON)
Set-GoogleSpeechConfig -Credential "C:\path\to\service-account.json"
Get-GoogleSpeech -Language ja-JP | Format-Table Name, Gender
Set-GoogleSpeechConfig -Voice "ja-JP-Neural2-B"
```

### Windows

```powershell
# No API key needed. Add voices: Settings > Time & language > Speech
Get-WindowsSpeech
Set-WindowsSpeechConfig -Voice "Microsoft Haruka Desktop"
```

</details>

<details>
<summary>Common options</summary>

All `Out-*Speech` cmdlets accept pipeline input and share these patterns:

```powershell
# Pipeline
"Line 1", "Line 2" | Out-AzureSpeech

# Output device selection (Tab completion available)
Out-AzureSpeech "Hello" -OutputDevice "Speakers (Realtek)"
Set-SpeechConfig -OutputDevice "Speakers (Realtek)"   # persist

# Microphone selection
Read-AzureSpeech -Microphone "Headset Microphone"
Set-SpeechConfig -Microphone "Headset Microphone"     # persist

# Parameter > config priority for all settings
Out-AzureSpeech "Hello" -Key "temp-key" -Region "westus"  # one-time override
```

</details>

## Troubleshooting

<details>
<summary>Common issues</summary>

**"key not configured" / "credential not configured"**
Run the provider's `Set-*Config` cmdlet. See `Get-Help Set-AzureSpeechConfig -Full`.

**No microphone input**
```powershell
Get-Microphone       # List devices
Test-Microphone      # Check input level (> 30 = OK)
```

**Windows STT not recognizing language**
Install language pack: Settings > Time & language > Language & region > Add language > "Speech" feature.


</details>

## License

[MIT](LICENSE)

Third-party: [NAudio](https://github.com/naudio/NAudio) (MIT), [Azure Speech SDK](https://github.com/Azure-Samples/cognitive-services-speech-sdk) (MIT).