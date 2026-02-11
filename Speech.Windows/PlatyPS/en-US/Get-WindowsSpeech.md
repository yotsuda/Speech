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

### System.Speech.Synthesis.VoiceInfo

## NOTES
- Windows only. Shows Desktop and OneCore voices.

## RELATED LINKS

[Out-WindowsSpeech](Out-WindowsSpeech.md)

[Set-WindowsSpeechConfig](Set-WindowsSpeechConfig.md)