---
external help file: Speech.Windows.dll-Help.xml
Module Name: Speech.Windows
online version:
schema: 2.0.0
---

# Read-WindowsSpeech

## SYNOPSIS
Speech recognition using Windows SAPI (STT).

## SYNTAX

```
Read-WindowsSpeech [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>] [-Language <String>]
 [-Confidence <Double>] [-NoAutoStop] [-PassThru] [-Microphone <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Continuous offline STT using Windows Speech Recognition. Real-time hypothesis display. Enter to confirm, Backspace to delete last line, auto-stop after silence.

Requires language pack for non-English: Settings > Time & language > Language & region > Add language > Speech.

## EXAMPLES

### Example 1: Basic STT
```powershell
$text = Read-WindowsSpeech
```

### Example 2: Japanese
```powershell
$text = Read-WindowsSpeech -Language ja-JP
```

### Example 3: High confidence
```powershell
$text = Read-WindowsSpeech -Confidence 0.7
```

### Example 4: Detailed results
```powershell
$results = Read-WindowsSpeech -PassThru
$results | Format-Table Text, Confidence, Duration
```

## PARAMETERS

### -Language
Language (e.g., `en-US`, `ja-JP`). Defaults to system culture. Requires language pack. Tab completion supported.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: System culture
Accept pipeline input: False
Accept wildcard characters: False
```

### -InitialTimeoutSeconds
Wait for first speech (1-300). Default: 30.

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
Silence to auto-stop (1-60). Default: 3.

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

### -Confidence
Minimum confidence threshold (0.0-1.0). Default: 0.3.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0.3
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

### -PassThru
Return objects with Text, Confidence, Duration, Timestamp.

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
Recognized text. Returns empty string if no speech detected within the timeout.

### System.Management.Automation.PSObject[]
With `-Detailed`, returns objects with Text, Confidence properties.

## NOTES
- Offline, no API key. Backchannel filtering built-in.
- No audio input: check microphone with `Get-Microphone` and `Test-Microphone`.
- Speech recognition not available: install language pack via Settings > Time & language > Language & region.

## RELATED LINKS

[Out-WindowsSpeech](Out-WindowsSpeech.md)

[Set-WindowsSpeechConfig](Set-WindowsSpeechConfig.md)