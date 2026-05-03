---
external help file: Speech.Amazon.dll-Help.xml
Module Name: Speech.Amazon
online version: https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Set-AmazonSpeechConfig.md
schema: 2.0.0
---

# Set-AmazonSpeechConfig

## SYNOPSIS
Configure Amazon-specific speech settings.

## SYNTAX

```
Set-AmazonSpeechConfig [-Voice <String>] [-AccessKey <String>] [-SecretKey <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Persists Amazon Polly voice and AWS credentials/region to the user config file. Only specified parameters are updated. View saved values with `Get-SpeechConfig`.

## EXAMPLES

### Example 1: Set credentials and region
```powershell
Set-AmazonSpeechConfig -AccessKey "AKIA..." -SecretKey "..." -Region "us-east-1"
```

### Example 2: Set default voice
```powershell
Set-AmazonSpeechConfig -Voice "Mizuki"
```

### Example 3: Switch region only
```powershell
Set-AmazonSpeechConfig -Region "ap-northeast-1"
```

## PARAMETERS

### -Voice
Default Polly voice (e.g., `Joanna`, `Mizuki`). Tab completion supported.

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
AWS access key ID. Stored in the user config file.

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
AWS secret access key. Stored in the user config file.

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
AWS region (e.g., `us-east-1`, `ap-northeast-1`).

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
Confirmation message including the config file path.

## NOTES
- Credentials are stored in plain text in the user config file. Treat the file as a secret. Prefer per-call `-AccessKey`/`-SecretKey` for shared machines.
- Calling with no parameters emits a warning and changes nothing.

## RELATED LINKS

[Get-AmazonSpeech](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Get-AmazonSpeech.md)

[Out-AmazonSpeech](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Out-AmazonSpeech.md)

[Read-AmazonSpeech](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Read-AmazonSpeech.md)
