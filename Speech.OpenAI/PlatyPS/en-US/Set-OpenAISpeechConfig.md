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
Saves OpenAI settings to `~/.speech/config.json`. See EXAMPLES for initial setup workflow.

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
- Key stored in plaintext in config.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-OpenAISpeech](Get-OpenAISpeech.md)

[Out-OpenAISpeech](Out-OpenAISpeech.md)