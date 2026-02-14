# Generate-Help.ps1 - Generate PlatyPS help markdown for all Speech modules
# This is a one-time generation script. Edit the en-US/*.md files directly after.

$ErrorActionPreference = 'Stop'

function Write-HelpFile {
    param([string]$Path, [string]$Content)
    Set-Content -Path $Path -Value $Content -NoNewline -Encoding UTF8
    Write-Host "  [OK] $(Split-Path $Path -Leaf)" -ForegroundColor Green
}

# ============================================================
# Speech.Core
# ============================================================
Write-Host "`n=== Speech.Core ===" -ForegroundColor Cyan
$d = "C:\MyProj\Speech\Speech.Core\PlatyPS\en-US"

Write-HelpFile "$d\Speech.Core.md" @'
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
Settings are stored in `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`. Common settings (Rate, Volume, Language, Microphone, OutputDevice) apply across all providers. Provider-specific settings are managed by each provider module:

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
'@

Write-HelpFile "$d\Get-SpeechConfig.md" @'
---
external help file: Speech.Core.dll-Help.xml
Module Name: Speech.Core
online version:
schema: 2.0.0
---

# Get-SpeechConfig

## SYNOPSIS
Display the current speech configuration.

## SYNTAX

```
Get-SpeechConfig [-Path] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Displays all speech configuration settings stored in `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`. Shows common settings (Rate, Volume, Language, Microphone, OutputDevice) and provider-specific settings for each installed module. API keys are masked for security (only last 4 characters shown).

## EXAMPLES

### Example 1: View all settings
```powershell
Get-SpeechConfig
```

Displays all configuration sections. Only installed provider sections are shown.

### Example 2: Get configuration file path
```powershell
Get-SpeechConfig -Path
```

Returns the full path to the configuration file (e.g., `C:\Users\user\Documents\PowerShell\Modules\Speech\SpeechConfig.json`).

### Example 3: Open config in editor
```powershell
code (Get-SpeechConfig -Path)
```

## PARAMETERS

### -Path
Returns only the configuration file path instead of displaying settings. Useful for scripting or opening the file in an editor.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- The configuration file is created automatically when you first use any `Set-*` cmdlet.
- Provider sections are only displayed if the corresponding module is installed.
- API keys are masked: only the last 4 characters are visible.

## RELATED LINKS

[Set-SpeechConfig](Set-SpeechConfig.md)
'@

Write-HelpFile "$d\Set-SpeechConfig.md" @'
---
external help file: Speech.Core.dll-Help.xml
Module Name: Speech.Core
online version:
schema: 2.0.0
---

# Set-SpeechConfig

## SYNOPSIS
Set common speech configuration settings shared across all providers.

## SYNTAX

```
Set-SpeechConfig [-Rate <Double>] [-Volume <Int32>] [-Microphone <String>] [-OutputDevice <String>]
 [-Language <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Configures common speech settings that apply to all provider modules. Settings are persisted to `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`.

For provider-specific settings (API keys, voices, etc.), use the dedicated cmdlets:

- `Set-AzureSpeechConfig` - Azure Speech Services
- `Set-OpenAISpeechConfig` - OpenAI TTS/STT
- `Set-GoogleSpeechConfig` - Google Cloud Speech
- `Set-WindowsSpeechConfig` - Windows SAPI

## EXAMPLES

### Example 1: Set speech rate and volume
```powershell
Set-SpeechConfig -Rate 1.2 -Volume 80
```

### Example 2: Set preferred microphone
```powershell
Get-Microphone
Set-SpeechConfig -Microphone "Headset Microphone"
```

### Example 3: Set audio output device
```powershell
Set-SpeechConfig -OutputDevice "Speakers (Realtek)"
```

### Example 4: Set default language
```powershell
Set-SpeechConfig -Language ja-JP
```

Note: changing the language clears conflicting provider voice settings.

## PARAMETERS

### -Rate
Speech speed multiplier. 0.5 = half, 1.0 = normal, 2.0 = double. Applies to all Out-*Speech cmdlets.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Volume
Volume percentage (0-100). Applies to all Out-*Speech cmdlets.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Microphone
Preferred microphone device name. Use `Get-Microphone` to list devices. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputDevice
Preferred audio output device name. Tab completion lists available devices.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Language
Default language code (e.g., `en-US`, `ja-JP`). Used when no language is specified on individual commands.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- At least one parameter must be specified.
- Settings are merged with existing configuration.

## RELATED LINKS

[Get-SpeechConfig](Get-SpeechConfig.md)

[Get-Microphone](Get-Microphone.md)
'@

Write-HelpFile "$d\Get-Microphone.md" @'
---
external help file: Speech.Core.dll-Help.xml
Module Name: Speech.Core
online version:
schema: 2.0.0
---

# Get-Microphone

## SYNOPSIS
List available microphone input devices.

## SYNTAX

```
Get-Microphone [-All] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Enumerates audio input (microphone) devices using NAudio. Returns device index, name, and channel count. Use the device name with `-Microphone` on speech recognition cmdlets or `Set-SpeechConfig -Microphone`.

## EXAMPLES

### Example 1: List all microphones
```powershell
Get-Microphone
```

### Example 2: Set preferred microphone from list
```powershell
Get-Microphone | Select-Object -First 1 | ForEach-Object { Set-SpeechConfig -Microphone $_.Name }
```

## PARAMETERS

### -All
Reserved for future use. All devices are always listed.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### Speech.Core.MicrophoneInfo

## NOTES
- Windows only. Device names may be truncated (32 chars max).
- Device index 0 is typically the system default microphone.

## RELATED LINKS

[Test-Microphone](Test-Microphone.md)

[Set-SpeechConfig](Set-SpeechConfig.md)
'@

Write-HelpFile "$d\Test-Microphone.md" @'
---
external help file: Speech.Core.dll-Help.xml
Module Name: Speech.Core
online version:
schema: 2.0.0
---

# Test-Microphone

## SYNOPSIS
Test microphone input levels with a visual meter.

## SYNTAX

```
Test-Microphone [-TestDurationSeconds <Int32>] [-Microphone <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Records from the microphone for a specified duration and displays a real-time visual level meter. Reports whether audio was detected with diagnostic information.

Use before speech recognition (Read-*Speech) to verify the microphone works.

## EXAMPLES

### Example 1: Quick test (5 seconds)
```powershell
Test-Microphone
```

### Example 2: Test specific microphone for 10 seconds
```powershell
Test-Microphone -Microphone "Headset Microphone" -TestDurationSeconds 10
```

### Example 3: Troubleshooting flow
```powershell
Get-Microphone                                       # List devices
Test-Microphone -Microphone "Headset Microphone"     # Verify input
Set-SpeechConfig -Microphone "Headset Microphone"    # Save as default
```

## PARAMETERS

### -TestDurationSeconds
Test duration in seconds (1-60). Default: 5.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 5
Accept pipeline input: False
Accept wildcard characters: False
```

### -Microphone
Microphone device name. Tab completion supported. Uses config default if not specified.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object

## NOTES
- Level > 30: working properly. Level 10-30: volume low. Level < 10: check connection.

## RELATED LINKS

[Get-Microphone](Get-Microphone.md)

[Set-SpeechConfig](Set-SpeechConfig.md)
'@

Write-Host "Speech.Core: done" -ForegroundColor Green

# ============================================================
# Speech.Azure
# ============================================================
Write-Host "`n=== Speech.Azure ===" -ForegroundColor Cyan
$d = "C:\MyProj\Speech\Speech.Azure\PlatyPS\en-US"

Write-HelpFile "$d\Speech.Azure.md" @'
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
'@

Write-HelpFile "$d\Out-AzureSpeech.md" @'
---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Out-AzureSpeech

## SYNOPSIS
Speak text aloud using Azure Speech Services (TTS).

## SYNTAX

```
Out-AzureSpeech [-Text] <String> [-Language <String>] [-Voice <String>] [-Rate <Double>] [-Volume <Int32>]
 [-Pitch <Int32>] [-OutputDevice <String>] [-Key <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Synthesizes speech from text using Azure Cognitive Services and plays it through the audio device. Supports SSML prosody (rate, volume, pitch) and 400+ neural voices. Voice resolution priority: `-Voice` > config voice > `-Language` > config language > default (`en-US-JennyNeural`). Detects language mismatch between text and voice.

Requires Azure Speech key and region. Run `Set-AzureSpeechConfig -Key "..." -Region "..."` first.

## EXAMPLES

### Example 1: Simple TTS
```powershell
Out-AzureSpeech "Hello, how are you?"
```

### Example 2: Japanese
```powershell
Out-AzureSpeech "こんにちは、世界" -Language ja-JP
```

### Example 3: Specific voice with rate
```powershell
Out-AzureSpeech "Good morning" -Voice "en-US-GuyNeural" -Rate 0.8
```

### Example 4: Pipeline
```powershell
"Line 1", "Line 2" | Out-AzureSpeech
```

## PARAMETERS

### -Text
Text to speak. Accepts pipeline input.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Language
Language code (e.g., `en-US`, `ja-JP`). Auto-selects a voice for the language. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Voice
Azure neural voice name (e.g., `ja-JP-NanamiNeural`). Use `Get-AzureSpeech` to list. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Rate
Speech speed (0.5-2.0). 1.0 = normal.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Volume
Volume percentage (0-100).

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Pitch
Voice pitch in Hz (-50 to +50). 0 = normal.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputDevice
Audio output device. Tab completion lists available devices.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Key
Azure subscription key. Overrides config. Use `Set-AzureSpeechConfig -Key` to persist.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Region
Azure region (e.g., `eastus`, `japaneast`). Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### None

## NOTES
- Free tier (F0): 0.5M characters/month neural TTS.

## RELATED LINKS

[Get-AzureSpeech](Get-AzureSpeech.md)

[Read-AzureSpeech](Read-AzureSpeech.md)

[Set-AzureSpeechConfig](Set-AzureSpeechConfig.md)
'@

Write-HelpFile "$d\Get-AzureSpeech.md" @'
---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Get-AzureSpeech

## SYNOPSIS
List available Azure neural voices.

## SYNTAX

```
Get-AzureSpeech [-Locale <String>] [-Key <String>] [-Region <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieves available neural voices from Azure Speech Services. Filter by locale prefix. Voice list is cached for 30 minutes.

## EXAMPLES

### Example 1: List all voices
```powershell
Get-AzureSpeech
```

### Example 2: Japanese voices
```powershell
Get-AzureSpeech -Locale ja
```

### Example 3: Find English female voices
```powershell
Get-AzureSpeech -Locale en | Where-Object Gender -eq Female
```

### Example 4: Set a voice
```powershell
Get-AzureSpeech -Locale ja | Select-Object -First 1 | ForEach-Object { Set-AzureSpeechConfig -Voice $_.ShortName }
```

## PARAMETERS

### -Locale
Filter by locale prefix (e.g., `ja`, `en-US`). Case-insensitive.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Key
Azure subscription key. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Region
Azure region. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### Speech.Azure.AzureSpeechInfo

## NOTES
- Cached for 30 minutes. 400+ voices across 140+ languages.

## RELATED LINKS

[Out-AzureSpeech](Out-AzureSpeech.md)

[Set-AzureSpeechConfig](Set-AzureSpeechConfig.md)
'@

Write-HelpFile "$d\Read-AzureSpeech.md" @'
---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Read-AzureSpeech

## SYNOPSIS
Real-time speech recognition using Azure Speech Services (STT).

## SYNTAX

```
Read-AzureSpeech [-Language <String>] [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>]
 [-NoAutoStop] [-PassThru] [-Microphone <String>] [-Key <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Continuous speech recognition with real-time display. Shows hypothesis text as you speak. Press Enter to confirm, Backspace to delete last line. Auto-stops after silence (unless `-NoAutoStop`).

Requires Azure Speech key and region.

## EXAMPLES

### Example 1: Basic STT
```powershell
$text = Read-AzureSpeech
```

### Example 2: Japanese
```powershell
$text = Read-AzureSpeech -Language ja-JP
```

### Example 3: Long recording
```powershell
$text = Read-AzureSpeech -NoAutoStop -InitialTimeoutSeconds 120
```

### Example 4: Detailed results
```powershell
$results = Read-AzureSpeech -PassThru
$results | Format-Table Text, Duration, Timestamp
```

## PARAMETERS

### -Language
Recognition language (e.g., `en-US`, `ja-JP`). Defaults to system culture. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: System culture
Accept pipeline input: False
Accept wildcard characters: False
```

### -InitialTimeoutSeconds
Seconds to wait for first speech (1-300). Default: 30.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 30
Accept pipeline input: False
Accept wildcard characters: False
```

### -EndSilenceSeconds
Silence seconds to trigger auto-stop (1-60). Default: 3.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoAutoStop
Disable auto-stop. Recording until Enter or timeout.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Return objects with Text, Duration, Timestamp instead of single string.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Microphone
Microphone device name. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Key
Azure subscription key. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Region
Azure region. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

### System.Management.Automation.PSObject[]

## NOTES
- Real-time streaming recognition (not batch). Free tier: 5h/month.

## RELATED LINKS

[Out-AzureSpeech](Out-AzureSpeech.md)

[Set-AzureSpeechConfig](Set-AzureSpeechConfig.md)
'@

Write-HelpFile "$d\Set-AzureSpeechConfig.md" @'
---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Set-AzureSpeechConfig

## SYNOPSIS
Configure Azure-specific speech settings.

## SYNTAX

```
Set-AzureSpeechConfig [-Voice <String>] [-Pitch <Int32>] [-Key <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Saves Azure Speech settings to `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`.

### Initial Setup
```powershell
Set-AzureSpeechConfig -Key "your-key" -Region "eastus"
Get-AzureSpeech -Locale ja | Format-Table ShortName, Gender
Set-AzureSpeechConfig -Voice "ja-JP-NanamiNeural"
Get-SpeechConfig
```

## EXAMPLES

### Example 1: Set credentials
```powershell
Set-AzureSpeechConfig -Key "abc123" -Region "japaneast"
```

### Example 2: Set voice (Tab completion)
```powershell
Set-AzureSpeechConfig -Voice "en-US-JennyNeural"
```

### Example 3: Adjust pitch
```powershell
Set-AzureSpeechConfig -Pitch 10
```

## PARAMETERS

### -Voice
Default Azure neural voice. Tab completion lists voices from API.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Pitch
Default pitch in Hz (-50 to +50).

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Key
Azure Speech subscription key. From Azure Portal > Speech resource > Keys and Endpoint.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Region
Azure region (e.g., `eastus`, `japaneast`, `westeurope`).

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- Key is stored in plaintext in config. Azure free tier (F0): 0.5M chars TTS + 5h STT/month.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-AzureSpeech](Get-AzureSpeech.md)

[Out-AzureSpeech](Out-AzureSpeech.md)
'@

Write-Host "Speech.Azure: done" -ForegroundColor Green

# ============================================================
# Speech.OpenAI
# ============================================================
Write-Host "`n=== Speech.OpenAI ===" -ForegroundColor Cyan
$d = "C:\MyProj\Speech\Speech.OpenAI\PlatyPS\en-US"

Write-HelpFile "$d\Speech.OpenAI.md" @'
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
'@

Write-HelpFile "$d\Out-OpenAISpeech.md" @'
---
external help file: Speech.OpenAI.dll-Help.xml
Module Name: Speech.OpenAI
online version:
schema: 2.0.0
---

# Out-OpenAISpeech

## SYNOPSIS
Speak text aloud using OpenAI TTS API.

## SYNTAX

```
Out-OpenAISpeech [-Text] <String> [-Voice <String>] [-Model <String>] [-Speed <Double>]
 [-ApiKey <String>] [-OutputDevice <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Converts text to speech via OpenAI Audio API and plays MP3 audio locally. Voices are multilingual (no language parameter needed).

Requires API key. Run `Set-OpenAISpeechConfig -Key "sk-..."` first.

## EXAMPLES

### Example 1: Simple TTS
```powershell
Out-OpenAISpeech "Hello, how are you?"
```

### Example 2: Specific voice
```powershell
Out-OpenAISpeech "Good morning" -Voice nova
```

### Example 3: High-quality slow
```powershell
Out-OpenAISpeech "Important" -Model tts-1-hd -Speed 0.8
```

### Example 4: Pipeline
```powershell
Get-Content notes.txt | Out-OpenAISpeech
```

## PARAMETERS

### -Text
Text to speak. Accepts pipeline input.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Voice
Voice: alloy, ash, ballad, coral, echo, fable, nova, onyx, sage, shimmer, verse. Tab completion supported. Default: alloy.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Model
TTS model: tts-1, tts-1-hd, gpt-4o-mini-tts. Tab completion supported. Default: tts-1.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Speed
Speed multiplier (0.25-4.0). 1.0 = normal.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ApiKey
OpenAI API key. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputDevice
Audio output device. Tab completion lists available devices.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### None

## NOTES
- Voices are multilingual. Pricing: tts-1 ~$15/1M chars, tts-1-hd ~$30/1M chars.

## RELATED LINKS

[Get-OpenAISpeech](Get-OpenAISpeech.md)

[Set-OpenAISpeechConfig](Set-OpenAISpeechConfig.md)
'@

Write-HelpFile "$d\Get-OpenAISpeech.md" @'
---
external help file: Speech.OpenAI.dll-Help.xml
Module Name: Speech.OpenAI
online version:
schema: 2.0.0
---

# Get-OpenAISpeech

## SYNOPSIS
List available OpenAI TTS voices.

## SYNTAX

```
Get-OpenAISpeech [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Returns available OpenAI TTS voices with descriptions. No API key needed (built-in list).

## EXAMPLES

### Example 1: List voices
```powershell
Get-OpenAISpeech
```

### Example 2: Set a voice
```powershell
Set-OpenAISpeechConfig -Voice "nova"
```

## PARAMETERS

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### Speech.OpenAI.OpenAISpeechInfo

## NOTES
- Static list, no API call.

## RELATED LINKS

[Out-OpenAISpeech](Out-OpenAISpeech.md)

[Set-OpenAISpeechConfig](Set-OpenAISpeechConfig.md)
'@

Write-HelpFile "$d\Read-OpenAISpeech.md" @'
---
external help file: Speech.OpenAI.dll-Help.xml
Module Name: Speech.OpenAI
online version:
schema: 2.0.0
---

# Read-OpenAISpeech

## SYNOPSIS
Speech recognition using OpenAI Whisper (STT).

## SYNTAX

```
Read-OpenAISpeech [-ApiKey <String>] [-Language <String>] [-Model <String>] [-Microphone <String>]
 [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>] [-NoAutoStop]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Records audio locally, then sends to Whisper API for batch transcription (not real-time). Enter to stop recording, or auto-stops after silence.

Requires API key. Run `Set-OpenAISpeechConfig -Key "sk-..."` first.

## EXAMPLES

### Example 1: Basic STT
```powershell
$text = Read-OpenAISpeech
```

### Example 2: Japanese
```powershell
$text = Read-OpenAISpeech -Language ja
```

### Example 3: Long recording
```powershell
$text = Read-OpenAISpeech -NoAutoStop -InitialTimeoutSeconds 300
```

## PARAMETERS

### -ApiKey
OpenAI API key. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Language
Language hint (ISO 639-1: en, ja, de, fr, etc.). Tab completion supported. Improves accuracy.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: System culture
Accept pipeline input: False
Accept wildcard characters: False
```

### -Model
Whisper model. Default: whisper-1. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Microphone
Microphone device name. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InitialTimeoutSeconds
Seconds to wait for first sound (1-300). Default: 30.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 30
Accept pipeline input: False
Accept wildcard characters: False
```

### -EndSilenceSeconds
Silence seconds to auto-stop (1-60). Default: 3.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoAutoStop
Disable auto-stop.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- Windows only. Batch mode (not real-time). Pricing: ~$0.006/min. Whisper supports 57+ languages.

## RELATED LINKS

[Out-OpenAISpeech](Out-OpenAISpeech.md)

[Set-OpenAISpeechConfig](Set-OpenAISpeechConfig.md)
'@

Write-HelpFile "$d\Set-OpenAISpeechConfig.md" @'
---
external help file: Speech.OpenAI.dll-Help.xml
Module Name: Speech.OpenAI
online version:
schema: 2.0.0
---

# Set-OpenAISpeechConfig

## SYNOPSIS
Configure OpenAI-specific speech settings.

## SYNTAX

```
Set-OpenAISpeechConfig [-Voice <String>] [-Model <String>] [-STTModel <String>] [-Key <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Saves OpenAI settings to `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`.

### Setup
```powershell
Set-OpenAISpeechConfig -Key "sk-..."
Get-OpenAISpeech
Set-OpenAISpeechConfig -Voice "nova" -Model "tts-1"
Get-SpeechConfig
```

## EXAMPLES

### Example 1: Set API key
```powershell
Set-OpenAISpeechConfig -Key "sk-proj-abc..."
```

### Example 2: Set voice and model
```powershell
Set-OpenAISpeechConfig -Voice "nova" -Model "tts-1-hd"
```

### Example 3: Set STT model
```powershell
Set-OpenAISpeechConfig -STTModel "whisper-1"
```

## PARAMETERS

### -Voice
Default TTS voice. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Model
Default TTS model (tts-1, tts-1-hd, gpt-4o-mini-tts). Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -STTModel
Default STT model for Read-OpenAISpeech. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Key
OpenAI API key (starts with `sk-`). From https://platform.openai.com/api-keys.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- Key stored in plaintext in config.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-OpenAISpeech](Get-OpenAISpeech.md)

[Out-OpenAISpeech](Out-OpenAISpeech.md)
'@

Write-Host "Speech.OpenAI: done" -ForegroundColor Green

# ============================================================
# Speech.Google
# ============================================================
Write-Host "`n=== Speech.Google ===" -ForegroundColor Cyan
$d = "C:\MyProj\Speech\Speech.Google\PlatyPS\en-US"

Write-HelpFile "$d\Speech.Google.md" @'
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
'@

Write-HelpFile "$d\Out-GoogleSpeech.md" @'
---
external help file: Speech.Google.dll-Help.xml
Module Name: Speech.Google
online version:
schema: 2.0.0
---

# Out-GoogleSpeech

## SYNOPSIS
Speak text aloud using Google Cloud TTS.

## SYNTAX

```
Out-GoogleSpeech [-Text] <String> [-Voice <String>] [-Language <String>] [-Rate <Double>] [-Pitch <Double>]
 [-Credential <String>] [-OutputDevice <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Converts text to speech via Google Cloud TTS API. Supports Standard, WaveNet, Neural2 voices. Requires credential file. Run `Set-GoogleSpeechConfig -Credential "path"` first.

## EXAMPLES

### Example 1: Simple TTS
```powershell
Out-GoogleSpeech "Hello!"
```

### Example 2: Japanese
```powershell
Out-GoogleSpeech "おはよう" -Voice "ja-JP-Neural2-B" -Language ja-JP
```

### Example 3: Adjust rate and pitch
```powershell
Out-GoogleSpeech "Slow and deep" -Rate 0.8 -Pitch -5.0
```

## PARAMETERS

### -Text
Text to speak. Accepts pipeline input.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Voice
Google voice name (e.g., `ja-JP-Neural2-B`). Tab completion supported (API-backed, 30-min cache).

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Language
Language code (e.g., `en-US`, `ja-JP`). Extracted from voice name if omitted. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Rate
Speed (0.25-4.0). 1.0 = normal.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Pitch
Pitch in semitones (-20.0 to +20.0). 0 = normal.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
Path to Google Cloud service account JSON file. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputDevice
Audio output device. Tab completion lists available devices.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### None

## NOTES
- Credential JSON must not be committed to git. Standard voices free 4M chars/month.

## RELATED LINKS

[Get-GoogleSpeech](Get-GoogleSpeech.md)

[Set-GoogleSpeechConfig](Set-GoogleSpeechConfig.md)
'@

Write-HelpFile "$d\Get-GoogleSpeech.md" @'
---
external help file: Speech.Google.dll-Help.xml
Module Name: Speech.Google
online version:
schema: 2.0.0
---

# Get-GoogleSpeech

## SYNOPSIS
List available Google Cloud TTS voices.

## SYNTAX

```
Get-GoogleSpeech [-Language <String>] [-Credential <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Lists voices from Google Cloud TTS API. Filter by language. Cached 30 minutes. Requires credential file.

## EXAMPLES

### Example 1: All voices
```powershell
Get-GoogleSpeech
```

### Example 2: Japanese voices
```powershell
Get-GoogleSpeech -Language ja-JP
```

### Example 3: WaveNet voices
```powershell
Get-GoogleSpeech -Language en-US | Where-Object Name -like "*Wavenet*"
```

## PARAMETERS

### -Language
Filter by language (e.g., `ja-JP`). Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
Path to credential JSON. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Management.Automation.PSObject

## NOTES
- Cached 30 min. Voice types: Standard, WaveNet, Neural2.

## RELATED LINKS

[Out-GoogleSpeech](Out-GoogleSpeech.md)

[Set-GoogleSpeechConfig](Set-GoogleSpeechConfig.md)
'@

Write-HelpFile "$d\Read-GoogleSpeech.md" @'
---
external help file: Speech.Google.dll-Help.xml
Module Name: Speech.Google
online version:
schema: 2.0.0
---

# Read-GoogleSpeech

## SYNOPSIS
Speech recognition using Google Cloud STT.

## SYNTAX

```
Read-GoogleSpeech [-Language <String>] [-Credential <String>] [-Microphone <String>]
 [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>] [-NoAutoStop]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Records audio then sends to Google Cloud STT for batch transcription. Requires credential file with Speech-to-Text API enabled.

## EXAMPLES

### Example 1: Japanese STT
```powershell
$text = Read-GoogleSpeech -Language ja-JP
```

### Example 2: Specific microphone
```powershell
$text = Read-GoogleSpeech -Language en-US -Microphone "Headset Microphone"
```

### Example 3: Long recording
```powershell
$text = Read-GoogleSpeech -NoAutoStop -InitialTimeoutSeconds 120
```

## PARAMETERS

### -Language
Language (e.g., `ja-JP`, `en-US`). Default: ja-JP. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: ja-JP
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
Path to credential JSON. Overrides config.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Microphone
Microphone device name. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InitialTimeoutSeconds
Wait seconds for first sound (1-120). Default: 30.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 30
Accept pipeline input: False
Accept wildcard characters: False
```

### -EndSilenceSeconds
Silence seconds to auto-stop (1-30). Default: 3.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoAutoStop
Disable auto-stop.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- Batch mode. Windows only. Free 60 min/month. Audio: 16kHz mono.

## RELATED LINKS

[Out-GoogleSpeech](Out-GoogleSpeech.md)

[Set-GoogleSpeechConfig](Set-GoogleSpeechConfig.md)
'@

Write-HelpFile "$d\Set-GoogleSpeechConfig.md" @'
---
external help file: Speech.Google.dll-Help.xml
Module Name: Speech.Google
online version:
schema: 2.0.0
---

# Set-GoogleSpeechConfig

## SYNOPSIS
Configure Google Cloud-specific speech settings.

## SYNTAX

```
Set-GoogleSpeechConfig [-Voice <String>] [-Credential <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Saves Google Cloud settings to `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`.

### Setup
```powershell
Set-GoogleSpeechConfig -Credential "C:\path\to\credential.json"
Get-GoogleSpeech -Language ja-JP | Format-Table Name, Gender
Set-GoogleSpeechConfig -Voice "ja-JP-Neural2-B"
Get-SpeechConfig
```

## EXAMPLES

### Example 1: Set credential
```powershell
Set-GoogleSpeechConfig -Credential "C:\keys\my-project-sa.json"
```

### Example 2: Set voice
```powershell
Set-GoogleSpeechConfig -Voice "en-US-Wavenet-D"
```

## PARAMETERS

### -Voice
Default Google TTS voice. Tab completion supported (API-backed).

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
Path to service account JSON file. Must exist. **Do not commit to git.**

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- Credential path stored; file not embedded. Ensure .gitignore excludes credential JSON.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-GoogleSpeech](Get-GoogleSpeech.md)

[Out-GoogleSpeech](Out-GoogleSpeech.md)
'@

Write-Host "Speech.Google: done" -ForegroundColor Green

# ============================================================
# Speech.Windows
# ============================================================
Write-Host "`n=== Speech.Windows ===" -ForegroundColor Cyan
$d = "C:\MyProj\Speech\Speech.Windows\PlatyPS\en-US"

Write-HelpFile "$d\Speech.Windows.md" @'
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
'@

Write-HelpFile "$d\Out-WindowsSpeech.md" @'
---
external help file: Speech.Windows.dll-Help.xml
Module Name: Speech.Windows
online version:
schema: 2.0.0
---

# Out-WindowsSpeech

## SYNOPSIS
Speak text aloud using Windows SAPI (TTS).

## SYNTAX

```
Out-WindowsSpeech [-Text] <String> [-Voice <String>] [-Rate <Double>] [-Volume <Int32>]
 [-OutputDevice <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Synthesizes speech using Windows Speech API. Works offline, no API key needed. When OutputDevice is specified, renders to WAV then plays via NAudio.

## EXAMPLES

### Example 1: Simple TTS
```powershell
Out-WindowsSpeech "Hello!"
```

### Example 2: Japanese voice
```powershell
Out-WindowsSpeech "こんにちは" -Voice "Microsoft Haruka Desktop"
```

### Example 3: Slow and quiet
```powershell
Out-WindowsSpeech "Good morning" -Rate 0.7 -Volume 50
```

### Example 4: Pipeline
```powershell
"Line 1", "Line 2" | Out-WindowsSpeech
```

## PARAMETERS

### -Text
Text to speak. Accepts pipeline input.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Voice
Windows SAPI voice name. Tab completion lists installed voices.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Rate
Speed (0.5-2.0). 1.0 = normal.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Volume
Volume (0-100).

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputDevice
Audio output device. Tab completion available.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### None

## NOTES
- No API key or internet needed. Add voices: Settings > Time & language > Speech.

## RELATED LINKS

[Get-WindowsSpeech](Get-WindowsSpeech.md)

[Set-WindowsSpeechConfig](Set-WindowsSpeechConfig.md)
'@

Write-HelpFile "$d\Get-WindowsSpeech.md" @'
---
external help file: Speech.Windows.dll-Help.xml
Module Name: Speech.Windows
online version:
schema: 2.0.0
---

# Get-WindowsSpeech

## SYNOPSIS
List installed Windows SAPI voices.

## SYNTAX

```
Get-WindowsSpeech [-Culture <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Returns installed SAPI voice information (name, culture, gender, age).

## EXAMPLES

### Example 1: All voices
```powershell
Get-WindowsSpeech
```

### Example 2: Japanese voices
```powershell
Get-WindowsSpeech -Culture ja-JP
```

### Example 3: Set a voice
```powershell
Get-WindowsSpeech | Select-Object -First 1 | ForEach-Object { Set-WindowsSpeechConfig -Voice $_.Name }
```

## PARAMETERS

### -Culture
Filter by culture (e.g., `ja-JP`). Tab completion lists cultures of installed voices.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Speech.Synthesis.VoiceInfo

## NOTES
- Windows only. Shows Desktop and OneCore voices.

## RELATED LINKS

[Out-WindowsSpeech](Out-WindowsSpeech.md)

[Set-WindowsSpeechConfig](Set-WindowsSpeechConfig.md)
'@

Write-HelpFile "$d\Read-WindowsSpeech.md" @'
---
external help file: Speech.Windows.dll-Help.xml
Module Name: Speech.Windows
online version:
schema: 2.0.0
---

# Read-WindowsSpeech

## SYNOPSIS
Speech recognition using Windows SAPI (STT).

## SYNTAX

```
Read-WindowsSpeech [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>] [-Language <String>]
 [-Confidence <Double>] [-NoAutoStop] [-PassThru] [-Microphone <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Continuous offline STT using Windows Speech Recognition. Real-time hypothesis display. Enter to confirm, Backspace to delete last line, auto-stop after silence.

Requires language pack for non-English: Settings > Time & language > Language & region > Add language > Speech.

## EXAMPLES

### Example 1: Basic STT
```powershell
$text = Read-WindowsSpeech
```

### Example 2: Japanese
```powershell
$text = Read-WindowsSpeech -Language ja-JP
```

### Example 3: High confidence
```powershell
$text = Read-WindowsSpeech -Confidence 0.7
```

### Example 4: Detailed results
```powershell
$results = Read-WindowsSpeech -PassThru
$results | Format-Table Text, Confidence, Duration
```

## PARAMETERS

### -Language
Language (e.g., `en-US`, `ja-JP`). Defaults to system culture. Requires language pack. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: System culture
Accept pipeline input: False
Accept wildcard characters: False
```

### -InitialTimeoutSeconds
Wait for first speech (1-300). Default: 30.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 30
Accept pipeline input: False
Accept wildcard characters: False
```

### -EndSilenceSeconds
Silence to auto-stop (1-60). Default: 3.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -Confidence
Minimum confidence threshold (0.0-1.0). Default: 0.3.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0.3
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoAutoStop
Disable auto-stop.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Return objects with Text, Confidence, Duration, Timestamp.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Microphone
Microphone device name. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

### System.Management.Automation.PSObject[]

## NOTES
- Offline, no API key. Backchannel filtering built-in.

## RELATED LINKS

[Out-WindowsSpeech](Out-WindowsSpeech.md)

[Set-WindowsSpeechConfig](Set-WindowsSpeechConfig.md)
'@

Write-HelpFile "$d\Set-WindowsSpeechConfig.md" @'
---
external help file: Speech.Windows.dll-Help.xml
Module Name: Speech.Windows
online version:
schema: 2.0.0
---

# Set-WindowsSpeechConfig

## SYNOPSIS
Configure Windows-specific speech settings.

## SYNTAX

```
Set-WindowsSpeechConfig [-Voice <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Saves Windows SAPI voice setting to `~/Documents/PowerShell/Modules/Speech/SpeechConfig.json`.

### Setup
```powershell
Get-WindowsSpeech
Set-WindowsSpeechConfig -Voice "Microsoft Haruka Desktop"
Get-SpeechConfig
```

## EXAMPLES

### Example 1: Set voice
```powershell
Set-WindowsSpeechConfig -Voice "Microsoft Haruka Desktop"
```

### Example 2: Set first available voice
```powershell
Get-WindowsSpeech | Select-Object -First 1 | ForEach-Object { Set-WindowsSpeechConfig -Voice $_.Name }
```

## PARAMETERS

### -Voice
Default Windows SAPI voice. Tab completion lists installed voices.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.String

## NOTES
- No API key needed. Add voices: Settings > Time & language > Speech.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-WindowsSpeech](Get-WindowsSpeech.md)

[Out-WindowsSpeech](Out-WindowsSpeech.md)
'@

Write-Host "Speech.Windows: done" -ForegroundColor Green

Write-Host "`n=== All 25 help files generated ===" -ForegroundColor Green
