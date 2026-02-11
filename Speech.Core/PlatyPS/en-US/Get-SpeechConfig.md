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
Displays all speech configuration settings stored in `~/.speech/config.json`. Shows common settings (Rate, Volume, Language, Microphone, OutputDevice) and provider-specific settings for each installed module. API keys are masked for security (only last 4 characters shown).

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

Returns the full path to the configuration file (e.g., `C:\Users\user\.speech\config.json`).

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
- The configuration file is created automatically when you first use any `Set-*` cmdlet.
- Provider sections are only displayed if the corresponding module is installed.
- API keys are masked: only the last 4 characters are visible.

## RELATED LINKS

[Set-SpeechConfig](Set-SpeechConfig.md)