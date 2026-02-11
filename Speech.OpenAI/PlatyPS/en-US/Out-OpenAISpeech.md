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
- Voices are multilingual. Pricing: tts-1 ~$15/1M chars, tts-1-hd ~$30/1M chars.
- Error "OpenAI API key not configured": run `Set-OpenAISpeechConfig -Key "sk-..."`.

## RELATED LINKS

[Get-OpenAISpeech](Get-OpenAISpeech.md)

[Set-OpenAISpeechConfig](Set-OpenAISpeechConfig.md)