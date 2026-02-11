---
external help file: Speech.Google.dll-Help.xml
Module Name: Speech.Google
online version:
schema: 2.0.0
---

# Read-GoogleSpeech

## SYNOPSIS
Speech recognition using Google Cloud STT.

## SYNTAX

```
Read-GoogleSpeech [-Language <String>] [-Credential <String>] [-Microphone <String>]
 [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>] [-NoAutoStop]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Records audio then sends to Google Cloud STT for batch transcription. Requires credential file with Speech-to-Text API enabled.

## EXAMPLES

### Example 1: Japanese STT
```powershell
$text = Read-GoogleSpeech -Language ja-JP
```

### Example 2: Specific microphone
```powershell
$text = Read-GoogleSpeech -Language en-US -Microphone "Headset Microphone"
```

### Example 3: Long recording
```powershell
$text = Read-GoogleSpeech -NoAutoStop -InitialTimeoutSeconds 120
```

## PARAMETERS

### -Language
Language (e.g., `ja-JP`, `en-US`). Default: ja-JP. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: ja-JP
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
Path to credential JSON. Priority: parameter > config. Use `Set-GoogleSpeechConfig -Credential` to persist.

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
Microphone device name. Tab completion supported.

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
Wait seconds for first sound (1-120). Default: 30.

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
Silence seconds to auto-stop (1-30). Default: 3.

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
Disable auto-stop.

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
Recognized text. Returns empty string if no speech detected.

## NOTES
- Batch mode. Windows only. Free 60 min/month. Audio: 16kHz mono.
- No audio input: check microphone with `Get-Microphone` and `Test-Microphone`.
- Error "Google credential not configured": run `Set-GoogleSpeechConfig -Credential "path/to/key.json"`.

## RELATED LINKS

[Out-GoogleSpeech](Out-GoogleSpeech.md)

[Set-GoogleSpeechConfig](Set-GoogleSpeechConfig.md)