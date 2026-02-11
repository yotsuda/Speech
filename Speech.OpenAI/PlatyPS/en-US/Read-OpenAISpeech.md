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
OpenAI API key. Priority: parameter > config. Use `Set-OpenAISpeechConfig -Key` to persist.

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
Recognized text. Returns empty string if no speech detected or recording is empty.

## NOTES
- Windows only. Batch mode (not real-time). Pricing: ~$0.006/min. Whisper supports 57+ languages.
- No audio input: check microphone with `Get-Microphone` and `Test-Microphone`.
- Error "OpenAI API key not configured": run `Set-OpenAISpeechConfig -Key "sk-..."`.

## RELATED LINKS

[Out-OpenAISpeech](Out-OpenAISpeech.md)

[Set-OpenAISpeechConfig](Set-OpenAISpeechConfig.md)