# Speech

PowerShell modules for text-to-speech (TTS) and speech-to-text (STT) across multiple providers.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue)]() [![PowerShell 7.4+](https://img.shields.io/badge/PowerShell-7.4+-blue)]() [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## Providers

| Module | TTS | STT | Requires |
|--------|-----|-----|----------|
| **Speech.Windows** | Offline SAPI | Offline SAPI | Windows 10/11 |
| **Speech.Azure** | 400+ neural voices | Real-time streaming | Azure Speech key |
| **Speech.OpenAI** | 11 multilingual voices | Whisper (batch) | OpenAI API key |
| **Speech.Google** | Standard/WaveNet/Neural2 | Batch | Google Cloud credential JSON |
| **Speech.Amazon** | Neural/standard voices | Real-time streaming | AWS access key + secret key |
| **Speech.Core** | — | — | (shared config, microphone, output device) |

## Platform Support

| Cmdlet | Windows | Linux/macOS |
|--------|---------|-------------|
| `Out-*Speech` (all providers) | Yes | Yes |
| `Read-AzureSpeech` | Yes | Yes |
| `Read-GoogleSpeech` | Yes | Yes |
| `Read-AmazonSpeech` | Yes | Yes |
| `Read-WindowsSpeech` | Yes | No (SAPI) |
| `Read-OpenAISpeech` | Yes | No (NAudio WinMM) |

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

# Amazon
Set-AmazonSpeechConfig -AccessKey "AKIA..." -SecretKey "..." -Region "ap-northeast-1"
Out-AmazonSpeech "Hello" -Voice Joanna

# Speech recognition (all providers)
$text = Read-WindowsSpeech
$text = Read-AzureSpeech -Language ja-JP
$text = Read-OpenAISpeech -Language ja
$text = Read-GoogleSpeech -Language ja-JP
$text = Read-AmazonSpeech -Language ja-JP
```

## Installation & Configuration

```powershell
Install-PSResource Speech
```

With [PowerShell.MCP](https://github.com/yotsuda/PowerShell.MCP#readme), AI can configure everything for you:

```powershell
Install-PSResource PowerShell.MCP
claude mcp add PowerShell -s user -- "$(Get-MCPProxyPath)"
```

Then just ask:

```
Install the Az module and help me create an Azure Speech resource.
```
```
Help me set up OpenAI Speech. I don't have an API key yet.
```
```
Guide me through setting up Google Cloud Speech.
```
```
Help me set up Amazon Polly with my AWS credentials.
```
```
Say 'Hello world' using Windows Speech.
```

Windows SAPI works offline with zero configuration — the quickest way to get started.

Settings are stored in `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`. API keys are masked when displayed.

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
Get-AzureSpeech -Locale ja
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
Get-GoogleSpeech -Language ja-JP
Set-GoogleSpeechConfig -Voice "ja-JP-Neural2-B"
```

### Amazon Polly / Transcribe

```powershell
# Get credentials: AWS Console > IAM > Users > Create access key
# Free tier: 5M chars TTS + 60 min STT / month (first 12 months)
Set-AmazonSpeechConfig -AccessKey "AKIA..." -SecretKey "..." -Region "ap-northeast-1"
Get-AmazonSpeech -Language ja-JP
Set-AmazonSpeechConfig -Voice "Mizuki"
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

## AI Voice Conversation

With [PowerShell.MCP](https://github.com/yotsuda/PowerShell.MCP#readme) configured, AI can speak and listen through your speakers and microphone:

```
Let's have a voice conversation in English.
```
```
When I type 't', start listening and respond by voice.
```
```
Find me a good English voice and play a sample.
```

### Compatible MCP Clients

Any MCP-compatible client that supports PowerShell.MCP can use Speech modules:

- [Claude Code](https://docs.anthropic.com/en/docs/claude-code) (CLI)
- [Claude Desktop](https://claude.ai/download)
- [GitHub Copilot (VS Code)](https://code.visualstudio.com/docs/copilot/overview)
- Any other MCP-compatible client

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
<summary>All 24 cmdlets</summary>

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

**Speech.Amazon** — Amazon Polly / Transcribe
- `Out-AmazonSpeech` — TTS with neural/standard voices (`-Voice`, `-Language`, `-Rate`)
- `Read-AmazonSpeech` — Real-time streaming STT (`-Language`)
- `Get-AmazonSpeech` — List available voices (`-Language` to filter)
- `Set-AmazonSpeechConfig` — Set `-AccessKey`, `-SecretKey`, `-Region`, `-Voice`

**Speech.Windows** — Windows SAPI
- `Out-WindowsSpeech` — Offline TTS (`-Voice`, `-Rate`, `-Volume`)
- `Read-WindowsSpeech` — Offline STT (`-Language`, `-Confidence`, `-Detailed`)
- `Get-WindowsSpeech` — List installed SAPI voices (`-Culture` to filter)
- `Set-WindowsSpeechConfig` — Set `-Voice`

</details>

## Tab Completion

Most parameters support <kbd>Tab</kbd> or <kbd>Ctrl</kbd>+<kbd>Space</kbd> completion. Voice and language lists are fetched from each provider's API and cached for the session.

| Cmdlet | Tab-completable Parameters |
|--------|---------------------------|
| `Out-WindowsSpeech` | `-Voice`, `-OutputDevice` |
| `Out-AzureSpeech` | `-Language`, `-Voice`, `-OutputDevice` |
| `Out-OpenAISpeech` | `-Model`, `-Voice`, `-OutputDevice` |
| `Out-GoogleSpeech` | `-Language`, `-Voice`, `-OutputDevice` |
| `Out-AmazonSpeech` | `-Language`, `-Voice`, `-OutputDevice` |
| `Read-WindowsSpeech` | `-Culture`, `-Microphone` |
| `Read-AzureSpeech` | `-Language`, `-Microphone` |
| `Read-OpenAISpeech` | `-Language`, `-Model`, `-Microphone` |
| `Read-GoogleSpeech` | `-Language`, `-Microphone` |
| `Read-AmazonSpeech` | `-Language`, `-Microphone` |
| `Get-WindowsSpeech` | `-Culture` |
| `Get-AzureSpeech` | `-Locale` |
| `Get-GoogleSpeech` | `-Language` |
| `Get-AmazonSpeech` | `-Language` |
| `Set-*SpeechConfig` | `-Voice`, `-Microphone`, `-OutputDevice` |

```powershell
# Language narrows the voice list
Out-AzureSpeech "Hello" -Language <Tab> -Voice <Tab>
# → en-US-JennyNeural, en-US-GuyNeural, ...

Out-OpenAISpeech "Hello" -Voice <Tab>
# → alloy, ash, ballad, coral, echo, fable, nova, onyx, sage, shimmer, verse

Read-AzureSpeech -Language <Tab>
# → en-US, ja-JP, zh-CN, ...

Read-OpenAISpeech -Microphone <Tab>
# → Headset Microphone, Microphone Array, ...
```

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

Third-party: [NAudio](https://github.com/naudio/NAudio) (MIT), [Azure Speech SDK](https://github.com/Azure-Samples/cognitive-services-speech-sdk) (MIT), [AWS SDK for .NET](https://github.com/aws/aws-sdk-net) (Apache-2.0).
