---
external help file: Speech.Amazon.dll-Help.xml
Module Name: Speech.Amazon
online version: https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Get-AmazonSpeech.md
schema: 2.0.0
---

# Get-AmazonSpeech

## SYNOPSIS
List available Amazon Polly voices.

## SYNTAX

```
Get-AmazonSpeech [-Language <String>] [-AccessKey <String>] [-SecretKey <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Lists voices from Amazon Polly's `DescribeVoices` API. Filter by language. Requires AWS credentials and region.

## EXAMPLES

### Example 1: All voices
```powershell
Get-AmazonSpeech
```

### Example 2: Japanese voices
```powershell
Get-AmazonSpeech -Language ja-JP
```

### Example 3: Neural-capable voices
```powershell
Get-AmazonSpeech -Language en-US | Where-Object Engines -like "*neural*"
```

## PARAMETERS

### -Language
Filter by language code (e.g., `ja-JP`, `en-US`). Tab completion supported.

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

### -AccessKey
AWS access key ID. Overrides config.

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

### -SecretKey
AWS secret access key. Overrides config.

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
AWS region (e.g., `us-east-1`, `ap-northeast-1`). Overrides config.

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
Voice objects with properties: Id, Name, Gender, Language, LanguageName, Engines.
Use `Format-Table Name, Gender, Engines` or `Where-Object { $_.Language -like "ja-*" }` to filter.

## NOTES
- Engines column lists supported synthesis engines: `standard`, `neural`, `long-form`, `generative`.

## RELATED LINKS

[Out-AmazonSpeech](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Out-AmazonSpeech.md)

[Set-AmazonSpeechConfig](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Set-AmazonSpeechConfig.md)
