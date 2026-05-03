---
external help file: Speech.Amazon.dll-Help.xml
Module Name: Speech.Amazon
online version: https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Out-AmazonSpeech.md
schema: 2.0.0
---

# Out-AmazonSpeech

## SYNOPSIS
Speak text aloud using Amazon Polly.

## SYNTAX

```
Out-AmazonSpeech [-Text] <String> [-Voice <String>] [-Language <String>] [-Rate <Double>] [-AccessKey <String>]
 [-SecretKey <String>] [-Region <String>] [-OutputDevice <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Converts text to speech via Amazon Polly's `SynthesizeSpeech` API and plays MP3 output. Supports Standard, Neural, Long-Form, and Generative engines. Requires AWS credentials. Run `Set-AmazonSpeechConfig -AccessKey "..." -SecretKey "..." -Region "..."` first.

## EXAMPLES

### Example 1: Simple TTS
```powershell
Out-AmazonSpeech "Hello!"
```

### Example 2: Japanese
```powershell
Out-AmazonSpeech "おはよう" -Voice "Mizuki" -Language ja-JP
```

### Example 3: Adjust rate
```powershell
Out-AmazonSpeech "Slow speech" -Rate 0.8
```

### Example 4: Pipeline input
```powershell
"Hello", "Bonjour", "こんにちは" | Out-AmazonSpeech
```

## PARAMETERS

### -Text
Text to speak. Accepts pipeline input. SSML special characters (`&`, `<`, `>`, `"`, `'`) are escaped automatically.

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
Polly voice name (e.g., `Joanna`, `Mizuki`, `Matthew`). Tab completion supported. Default selected from UI culture (en→Joanna, ja→Mizuki, ko→Seoyeon, zh→Zhiyu, de→Vicki, fr→Lea, es→Lucia, it→Bianca, pt→Camila).

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

### -Language
Language code (e.g., `en-US`, `ja-JP`). Tab completion supported. Falls back to Common config language.

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

### -Rate
Speed (0.25-4.0). 1.0 = normal. Implemented via SSML `<prosody rate>`.

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

### -AccessKey
AWS access key ID. Priority: parameter > config. Use `Set-AmazonSpeechConfig -AccessKey` to persist.

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
AWS secret access key. Priority: parameter > config.

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
- Standard voices are free for the first 5M chars/month for 12 months. Neural/Long-Form/Generative cost more — check current Polly pricing.
- Error "Amazon credentials not specified": run `Set-AmazonSpeechConfig -AccessKey "..." -SecretKey "..." -Region "..."`.

## RELATED LINKS

[Get-AmazonSpeech](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Get-AmazonSpeech.md)

[Set-AmazonSpeechConfig](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Set-AmazonSpeechConfig.md)
