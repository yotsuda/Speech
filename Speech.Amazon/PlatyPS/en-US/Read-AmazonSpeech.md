---
external help file: Speech.Amazon.dll-Help.xml
Module Name: Speech.Amazon
online version: https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Read-AmazonSpeech.md
schema: 2.0.0
---

# Read-AmazonSpeech

## SYNOPSIS
Speech recognition using Amazon Transcribe Streaming.

## SYNTAX

```
Read-AmazonSpeech [-Language <String>] [-AccessKey <String>] [-SecretKey <String>] [-Region <String>]
 [-Microphone <String>] [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>] [-NoAutoStop]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Records 16 kHz / 16-bit / mono audio from the default (or specified) microphone, then sends it to Amazon Transcribe Streaming and returns the recognized text. Auto-stops after `EndSilenceSeconds` of silence (or press Enter). Requires AWS credentials.

## EXAMPLES

### Example 1: Default language
```powershell
$text = Read-AmazonSpeech
```

### Example 2: Japanese with longer silence threshold
```powershell
$text = Read-AmazonSpeech -Language ja-JP -EndSilenceSeconds 5
```

### Example 3: Manual stop only (press Enter)
```powershell
$text = Read-AmazonSpeech -NoAutoStop
```

### Example 4: Specific microphone
```powershell
$text = Read-AmazonSpeech -Microphone "USB Headset"
```

## PARAMETERS

### -Language
Language code (e.g., `ja-JP`, `en-US`). Default: Common config language, then `ja-JP`. Tab completion supported.

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
AWS region (e.g., `us-east-1`, `ap-northeast-1`). Must support Transcribe Streaming.

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

### -Microphone
Microphone device name. Tab completion lists available devices.

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

### -InitialTimeoutSeconds
Maximum seconds to wait for the first sound before giving up. Range 1-120. Default 30.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 30
Accept pipeline input: False
Accept wildcard characters: False
```

### -EndSilenceSeconds
Seconds of trailing silence that auto-stops recording. Range 1-30. Default 3. Ignored when `-NoAutoStop` is specified.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoAutoStop
Disable silence-based auto-stop. Recording continues until Enter is pressed.

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
Recognized text (trimmed). Empty string if nothing was detected.

## NOTES
- Free tier: 60 min/month for 12 months.
- Sound detection threshold: max 16-bit amplitude > 1500 marks "has sound". Adjust `EndSilenceSeconds` if cut-off is too aggressive.
- Transcribe Streaming is not available in every AWS region — check the AWS regional service list.

## RELATED LINKS

[Out-AmazonSpeech](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Out-AmazonSpeech.md)

[Set-AmazonSpeechConfig](https://github.com/yotsuda/Speech/blob/main/Speech.Amazon/PlatyPS/en-US/Set-AmazonSpeechConfig.md)
