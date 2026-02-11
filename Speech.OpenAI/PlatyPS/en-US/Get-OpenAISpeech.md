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

### Speech.OpenAI.OpenAISpeechInfo

## NOTES
- Static list, no API call.

## RELATED LINKS

[Out-OpenAISpeech](Out-OpenAISpeech.md)

[Set-OpenAISpeechConfig](Set-OpenAISpeechConfig.md)