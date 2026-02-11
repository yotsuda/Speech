---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Get-AzureSpeech

## SYNOPSIS
List available Azure neural voices.

## SYNTAX

```
Get-AzureSpeech [-Locale <String>] [-Key <String>] [-Region <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieves available neural voices from Azure Speech Services. Filter by locale prefix. Voice list is cached for 30 minutes.

## EXAMPLES

### Example 1: List all voices
```powershell
Get-AzureSpeech
```

### Example 2: Japanese voices
```powershell
Get-AzureSpeech -Locale ja
```

### Example 3: Find English female voices
```powershell
Get-AzureSpeech -Locale en | Where-Object Gender -eq Female
```

### Example 4: Set a voice
```powershell
Get-AzureSpeech -Locale ja | Select-Object -First 1 | ForEach-Object { Set-AzureSpeechConfig -Voice $_.ShortName }
```

## PARAMETERS

### -Locale
Filter by locale prefix (e.g., `ja`, `en-US`). Case-insensitive.

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
Azure subscription key. Overrides config.

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

### -Region
Azure region. Overrides config.

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

### Speech.Azure.AzureSpeechInfo
Voice objects with properties: ShortName, DisplayName, Gender, Locale, LocaleName, VoiceType, SampleRateHertz, WordsPerMinute, StyleList.
Use `Format-Table ShortName, DisplayName, Gender` or `Where-Object { $_.Locale -like "ja-*" }` to filter.

## NOTES
- Cached for 30 minutes. 400+ voices across 140+ languages.

## RELATED LINKS

[Out-AzureSpeech](Out-AzureSpeech.md)

[Set-AzureSpeechConfig](Set-AzureSpeechConfig.md)