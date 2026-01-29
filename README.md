# Speech

A PowerShell module providing unified voice synthesis (TTS) and recognition (STT) across Windows Speech API and Azure Speech Services.

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)]() [![PowerShell](https://img.shields.io/badge/PowerShell-7.0+-blue)]()

## Features

### 🔊 Text-to-Speech (TTS)
- **Windows Speech API** - Built-in SAPI support
- **Azure Speech Services** - Cloud-based neural voices
- **Background Playback** - Queue-based async audio playback

### 🎤 Speech-to-Text (STT)
- **Windows Speech Recognition** - Local speech recognition
- **Azure Speech Services** - Cloud-based continuous recognition
- **Enter to Stop** - Natural interaction (press Enter when done speaking)

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
git clone https://github.com/yourusername/Speech.git
cd Speech

# Build the project
dotnet build Speech/Speech.csproj -c Release

# Import the module
Import-Module .\Speech\bin\Release\net9.0\Speech.psd1
```

## Quick Start

### Text-to-Speech

```powershell
# Windows TTS
Out-WindowsSpeech "Hello, world!"

# Azure TTS with custom voice
Out-AzureSpeech "こんにちは" -Voice "ja-JP-NanamiNeural" -Key $env:AZURE_SPEECH_KEY -Region "japaneast"

# List available voices
Get-WindowsSpeech
Get-AzureSpeech -Key $env:AZURE_SPEECH_KEY -Region "japaneast"
```

### Speech-to-Text

```powershell
# Windows STT (single recognition)
$text = Read-WindowsSpeech

# Azure STT (continuous recognition, press Enter to stop)
$text = Read-AzureSpeech -Key $env:AZURE_SPEECH_KEY -Region "japaneast"
```

### Interactive Conversation Example

```powershell
while ($true) {
    Out-WindowsSpeech "How can I help you?"
    
    $input = Read-WindowsSpeech -TimeoutSeconds 30
    
    if (-not $input) { continue }
    
    if ($input -match "exit|quit|goodbye") {
        Out-WindowsSpeech "Goodbye!"
        break
    }
    
    # Process input...
}
```

## Configuration

Configuration is automatically saved to:
- **Windows**: `Documents\PowerShell\Modules\Speech\VoiceConfig.json`

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
    "Voice": "ja-JP-NanamiNeural"
  }
}
```

### Manage Configuration

```powershell
# View current configuration
Get-SpeechConfig

# Update configuration
Set-SpeechConfig -WindowsVoice "Microsoft David Desktop" -Rate 1.2
```

## Cmdlet Reference

### Windows Speech API

#### `Out-WindowsSpeech`
Synthesize speech using Windows SAPI.

```powershell
Out-WindowsSpeech [-Text] <string>
    [-Voice <string>]
    [-Rate <double>]   # 0.5 to 2.0
    [-Volume <int>]    # 0 to 100
```

#### `Get-WindowsSpeech`
List available Windows voices.

```powershell
Get-WindowsSpeech
```

#### `Read-WindowsSpeech`
Continuous speech recognition using Windows Speech API. Stops automatically after silence, or press Enter to stop immediately.

```powershell
Read-WindowsSpeech
    [-InitialTimeoutSeconds <int>]  # Before speech, Default: 30
    [-EndSilenceSeconds <int>]      # After speech, Default: 3
    [-Language <string>]            # Default: "ja-JP"
    [-Confidence <double>]          # 0.0 to 1.0, Default: 0.3
    [-NoAutoStop]                   # Require Enter to stop
    [-PassThru]                     # Return detailed results
```

### Azure Speech Services

#### `Out-AzureSpeech`
Synthesize speech using Azure TTS.

```powershell
Out-AzureSpeech [-Text] <string>
    -Key <string>
    -Region <string>
    [-Voice <string>]
    [-Rate <double>]   # 0.5 to 2.0
    [-Volume <int>]    # 0 to 100
    [-Pitch <int>]     # -50 to 50 Hz
```

#### `Get-AzureSpeech`
List available Azure neural voices.

```powershell
Get-AzureSpeech
    -Key <string>
    -Region <string>
    [-Locale <string>]  # Filter by locale (e.g., "ja-JP")
```

#### `Read-AzureSpeech`
Continuous speech recognition using Azure Speech Services. Stops automatically after silence, or press Enter to stop immediately.

```powershell
Read-AzureSpeech
    -Key <string>
    -Region <string>
    [-Language <string>]            # Default: "ja-JP"
    [-InitialTimeoutSeconds <int>]  # Before speech, Default: 30
    [-EndSilenceSeconds <int>]      # After speech, Default: 3
    [-NoAutoStop]                   # Require Enter to stop
    [-PassThru]                     # Return detailed results
```

### Utilities

#### `Get-SpeechConfig`
Display current configuration.

```powershell
Get-SpeechConfig
```

#### `Set-SpeechConfig`
Update configuration settings.

```powershell
Set-SpeechConfig
    [-WindowsVoice <string>]
    [-AzureKey <string>]
    [-AzureRegion <string>]
    [-AzureVoice <string>]
    [-Rate <double>]
    [-Volume <int>]
```

#### `Get-SpeechQueueState`
Get TTS queue status.

```powershell
Get-SpeechQueueState
```

#### `Clear-SpeechQueue`
Clear pending TTS queue.

```powershell
Clear-SpeechQueue
```

#### `Test-Microphone`
Test microphone input levels.

```powershell
Test-Microphone
```

## Troubleshooting

### Windows Speech Recognition Not Working
- Ensure Windows Speech Recognition is set up (Settings → Time & Language → Speech)
- Check microphone permissions
- Run `Test-Microphone` to verify input

### Azure TTS/STT Errors
- Verify your subscription key and region
- Check for language mismatch (e.g., Japanese text with English voice)
- Ensure your Azure Speech Services subscription is active

### Module Loading Issues
- Ensure .NET 9.0 runtime is installed
- Try `Import-Module` with `-Force` flag
- Check for DLL locking (restart PowerShell session)

## License

This project uses the following third-party libraries:
- **NAudio** - MIT License (see LICENSES/NAudio-LICENSE.txt)
- **Microsoft.CognitiveServices.Speech** - Microsoft Software License
- **System.Speech** - .NET Foundation
- **System.Management.Automation** - MIT License

## Credits

Developed with ❤️ by Yoshifumi Tsuda.

---

**Version**: 0.3.0
**Last Updated**: 2026-01-28