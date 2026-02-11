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
Saves Windows SAPI voice setting to `~/.speech/config.json`. See EXAMPLES for setup workflow.

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
- No API key needed. Add voices: Settings > Time & language > Speech.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-WindowsSpeech](Get-WindowsSpeech.md)

[Out-WindowsSpeech](Out-WindowsSpeech.md)