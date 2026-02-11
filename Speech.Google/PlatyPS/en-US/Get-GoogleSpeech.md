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

### System.Management.Automation.PSObject
Voice objects with properties: Name, Gender, Languages.
Use `Format-Table Name, Gender, Languages` or `Where-Object { $_.Languages -like "ja-*" }` to filter.

## NOTES
- Cached 30 min. Voice types: Standard, WaveNet, Neural2.

## RELATED LINKS

[Out-GoogleSpeech](Out-GoogleSpeech.md)

[Set-GoogleSpeechConfig](Set-GoogleSpeechConfig.md)