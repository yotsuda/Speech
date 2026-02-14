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
This is a common parameter. This implementation ignores it.

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