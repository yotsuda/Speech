---
external help file: Speech.Google.dll-Help.xml
Module Name: Speech.Google
online version:
schema: 2.0.0
---

# Out-GoogleSpeech

## SYNOPSIS
Speak text aloud using Google Cloud TTS.

## SYNTAX

```
Out-GoogleSpeech [-Text] <String> [-Voice <String>] [-Language <String>] [-Rate <Double>] [-Pitch <Double>]
 [-Credential <String>] [-OutputDevice <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Converts text to speech via Google Cloud TTS API. Supports Standard, WaveNet, Neural2 voices. Requires credential file. Run `Set-GoogleSpeechConfig -Credential "path"` first.

## EXAMPLES

### Example 1: Simple TTS
```powershell
Out-GoogleSpeech "Hello!"
```

### Example 2: Japanese
```powershell
Out-GoogleSpeech "おはよう" -Voice "ja-JP-Neural2-B" -Language ja-JP
```

### Example 3: Adjust rate and pitch
```powershell
Out-GoogleSpeech "Slow and deep" -Rate 0.8 -Pitch -5.0
```

## PARAMETERS

### -Text
Text to speak. Accepts pipeline input.

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
Google voice name (e.g., `ja-JP-Neural2-B`). Tab completion supported (API-backed, 30-min cache).

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
Language code (e.g., `en-US`, `ja-JP`). Extracted from voice name if omitted. Tab completion supported.

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
Speed (0.25-4.0). 1.0 = normal.

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

### -Pitch
Pitch in semitones (-20.0 to +20.0). 0 = normal.

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

### -Credential
Path to Google Cloud service account JSON file. Priority: parameter > config. Use `Set-GoogleSpeechConfig -Credential` to persist.

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
- Credential JSON must not be committed to git. Standard voices free 4M chars/month.
- Error "Google credential not configured": run `Set-GoogleSpeechConfig -Credential "path/to/key.json"`.

## RELATED LINKS

[Get-GoogleSpeech](Get-GoogleSpeech.md)

[Set-GoogleSpeechConfig](Set-GoogleSpeechConfig.md)