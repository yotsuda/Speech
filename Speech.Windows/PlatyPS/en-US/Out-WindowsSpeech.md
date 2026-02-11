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

### System.String

## OUTPUTS

### None

## NOTES
- No API key or internet needed. Add voices: Settings > Time & language > Speech.
- No voices found: ensure speech packs are installed (`Get-WindowsSpeech` to check).

## RELATED LINKS

[Get-WindowsSpeech](Get-WindowsSpeech.md)

[Set-WindowsSpeechConfig](Set-WindowsSpeechConfig.md)