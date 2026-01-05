# Voice

A PowerShell module providing unified voice synthesis (TTS) and recognition (STT) across Windows Speech API and Azure Speech Services.

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)]() [![PowerShell](https://img.shields.io/badge/PowerShell-7.0+-blue)]()

## Features

### 🔊 Text-to-Speech (TTS)
- **Windows Speech API** - Built-in SAPI support
- **Azure Speech Services** - Cloud-based neural voices
- **Background Playback** - Queue-based async audio playback
- **Barge-in Support** - Interrupt TTS when user speaks

### 🎤 Speech-to-Text (STT)
- **Windows Speech Recognition** - Local speech recognition
- **Background Listening** - Continuous voice monitoring
- **Backchannel Detection** - Filters out acknowledgments ("uh-huh", "yeah", etc.)
- **Configurable Confidence** - Adjustable recognition thresholds

### ⚙️ Configuration
- **Smart Defaults** - Automatic locale-based voice selection
- **Persistent Config** - JSON-based settings storage
- **Flexible Parameters** - Consistent rate/volume/voice controls

## Installation

### Prerequisites
- Windows 10/11 (for Windows Speech API)
- PowerShell 7.0 or higher
- .NET 9.0 Runtime
- Azure Speech Services subscription (optional, for Azure features)

### Build from Source

```powershell
# Clone the repository
git clone https://github.com/yourusername/Voice.git
cd Voice

# Build the project
dotnet build Voice/Voice.csproj -c Release

# Import the module
Import-Module .\Voice\bin\Release\net9.0\Voice.psd1
```

## Quick Start

### Text-to-Speech

```powershell
# Simple TTS (Windows)
Out-WindowsVoice "Hello, world!"

# Azure TTS with custom voice
Out-AzureVoice "こんにちは" -Voice "ja-JP-NanamiNeural" -Key "YOUR_KEY" -Region "japaneast"

# List available voices
Get-WindowsVoice
Get-AzureVoice -Key "YOUR_KEY" -Region "japaneast"
```

### Speech-to-Text

```powershell
# Background listening (recommended)
Start-VoiceListening
$input = Get-VoiceInput -WaitSeconds 30
Stop-VoiceListening

# Single recognition
Invoke-WindowsVoiceRecognition -TimeoutSeconds 10
```

### Interactive Conversation

```powershell
# Start background listening
Start-VoiceListening

# Conversation loop
while ($true) {
    # TTS output (non-blocking)
    Out-WindowsVoice "How can I help you?"

    # STT input (blocking until speech detected)
    $input = Get-VoiceInput -WaitSeconds 60 -IgnoreBackchannel

    if (-not $input) { continue }

    if ($input -match "exit|quit|goodbye") {
        Out-WindowsVoice "Goodbye!"
        Start-Sleep -Seconds 2
        break
    }

    # Process input and generate response
    # ...
}

Stop-VoiceListening
```

## Configuration

Configuration is automatically saved to:
- **Windows**: `Documents\PowerShell\Modules\Voice\VoiceConfig.json`
- **Unix**: `~/.local/share/powershell/Modules/Voice/VoiceConfig.json`

### Example Configuration

```json
{
  "Common": {
    "Rate": 1.0,
    "Volume": 100
  },
  "Windows": {
    "Voice": "Microsoft Zira Desktop"
  },
  "Azure": {
    "Key": "your-azure-key",
    "Region": "japaneast",
    "Voice": "ja-JP-NanamiNeural",
    "Pitch": 0
  }
}
```

### Manage Configuration

```powershell
# View current configuration
Get-VoiceConfig

# Settings are automatically saved when you specify parameters
Out-WindowsVoice "Test" -Voice "Microsoft David Desktop" -Rate 1.5
# Voice and Rate are now saved for future use
```

## Cmdlet Reference

### Windows Speech API

#### `Out-WindowsVoice`
Synthesize speech using Windows SAPI.

```powershell
Out-WindowsVoice [-Text] <string>
    [-Voice <string>]
    [-Rate <int>]     # -10 to 10
    [-Volume <int>]   # 0 to 100
    [-Wait]
```

#### `Get-WindowsVoice`
List available Windows voices.

```powershell
Get-WindowsVoice
```

#### `Invoke-WindowsVoiceRecognition`
Perform single speech recognition.

```powershell
Invoke-WindowsVoiceRecognition
    [-TimeoutSeconds <int>]
    [-Language <string>]     # Default: "en-US"
    [-Confidence <double>]   # 0.0 to 1.0
```

#### `Wait-WindowsInput`
Wait for speech input with barge-in support.

```powershell
Wait-WindowsInput
    [-TimeoutSeconds <int>]
    [-Language <string>]
    [-Confidence <double>]
    [-CancelOnSpeech]        # Enable barge-in
```

### Azure Speech Services

#### `Out-AzureVoice`
Synthesize speech using Azure TTS.

```powershell
Out-AzureVoice [-Text] <string>
    -Key <string>
    -Region <string>
    [-Voice <string>]
    [-Rate <double>]   # 0.5 to 2.0
    [-Volume <int>]    # 0 to 100
    [-Pitch <int>]     # -50 to 50 Hz
```

#### `Get-AzureVoice`
List available Azure neural voices.

```powershell
Get-AzureVoice
    -Key <string>
    -Region <string>
    [-Locale <string>]  # Filter by locale (e.g., "ja-JP")
```

### Background Voice Recognition

#### `Start-VoiceListening`
Start background speech recognition.

```powershell
Start-VoiceListening
    [-Culture <string>]  # Default: "en-US"
```

#### `Get-VoiceInput`
Get recognized speech from background listener.

```powershell
Get-VoiceInput
    [-WaitSeconds <int>]       # 0 = poll, >0 = blocking wait
    [-IgnoreBackchannel]       # Filter out acknowledgments
    [-PassThru]                # Return detailed result object
```

#### `Stop-VoiceListening`
Stop background speech recognition.

```powershell
Stop-VoiceListening
```

### Utilities

#### `Get-VoiceConfig`
Display current configuration.

```powershell
Get-VoiceConfig
```

#### `Get-VoiceQueueState`
Get TTS queue status.

```powershell
Get-VoiceQueueState
```

#### `Clear-VoiceQueue`
Clear pending TTS queue.

```powershell
Clear-VoiceQueue [-Confirm]
```

#### `Test-Microphone`
Test microphone input levels.

```powershell
Test-Microphone
```

## Architecture

### Core Components

```
Voice/
├── Core/
│   ├── VoiceState.cs              # Global TTS queue management
│   └── WindowsVoiceRequest.cs     # Windows TTS implementation
├── Cmdlets/
│   ├── Windows/                   # Windows Speech API cmdlets
│   │   ├── Out-WindowsVoice.cs
│   │   ├── Get-WindowsVoice.cs
│   │   ├── Invoke-WindowsVoiceRecognition.cs
│   │   ├── Wait-WindowsInput.cs
│   │   └── WindowsAudioManager.cs # Shared Windows resources
│   ├── Azure/                     # Azure Speech Services cmdlets
│   │   ├── Out-AzureVoice.cs
│   │   ├── Get-AzureVoice.cs
│   │   └── AzureAudioManager.cs   # Azure REST API client
│   ├── Voice/                     # Service-independent voice features
│   │   ├── VoiceRecognitionState.cs
│   │   ├── Start-VoiceListening.cs
│   │   ├── Get-VoiceInput.cs
│   │   └── Stop-VoiceListening.cs
│   └── Common/                    # Shared utilities
│       ├── ConfigManager.cs       # Config persistence
│       ├── Get-VoiceConfig.cs
│       ├── Get-VoiceQueueState.cs
│       ├── Clear-VoiceQueue.cs
│       └── Test-Microphone.cs
└── Voice.psd1                  # Module manifest
```

### Design Patterns

- **Queue-based TTS**: Background async playback prevents UI blocking
- **Static Managers**: Singleton pattern for shared audio resources
- **Event-driven STT**: Continuous recognition with event handlers
- **Smart Defaults**: Locale-aware configuration initialization

## Barge-in Feature

Voice supports interrupting TTS playback when the user speaks:

```powershell
# Method 1: Using Wait-WindowsInput
Out-WindowsVoice "This is a long message that can be interrupted..."
$input = Wait-WindowsInput -CancelOnSpeech

# Method 2: Using background listening
Start-VoiceListening
Out-WindowsVoice "Long TTS message..."
$input = Get-VoiceInput -WaitSeconds 30
# If user speaks, they can interrupt naturally
```

### Backchannel Detection

The module automatically detects and can filter short acknowledgments:
- **Japanese**: うん, ええ, はい, へー, ほー, なるほど, そうですか
- **English**: uh-huh, yeah, okay, right, i see, mm-hmm, oh, yes

```powershell
# Ignore backchannels, only return meaningful input
$input = Get-VoiceInput -WaitSeconds 30 -IgnoreBackchannel
```

## Troubleshooting

### Windows Speech Recognition Not Working
- Ensure Windows Speech Recognition is set up (Settings → Time & Language → Speech)
- Check microphone permissions
- Run `Test-Microphone` to verify input

### Azure TTS Errors
- Verify your subscription key and region
- Check for language mismatch (e.g., Japanese text with English voice)
- Debug SSML files are saved to `%TEMP%\Voice-debug-*.xml` on errors

### Module Loading Issues
- Ensure .NET 9.0 runtime is installed
- Try `Import-Module` with `-Force` flag
- Check for DLL locking (restart PowerShell session)

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

### Development

```powershell
# Build in debug mode
dotnet build Voice/Voice.csproj -c Debug

# Run tests
.\Test-Phase2-Integration.ps1
```

## License

This project uses the following third-party libraries:
- **NAudio** - MIT License (see LICENSES/NAudio-LICENSE.txt)
- **System.Speech** - .NET Foundation
- **System.Management.Automation** - MIT License

## Credits

Developed with ❤️ by the Voice team.

---

**Version**: 0.1.0
**Last Updated**: 2025-10-07
