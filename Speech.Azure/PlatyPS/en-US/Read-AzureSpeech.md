---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Read-AzureSpeech

## SYNOPSIS
Real-time speech recognition using Azure Speech Services (STT).

## SYNTAX

```
Read-AzureSpeech [-Language <String>] [-InitialTimeoutSeconds <Int32>] [-EndSilenceSeconds <Int32>]
 [-NoAutoStop] [-PassThru] [-Microphone <String>] [-Key <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Continuous speech recognition with real-time display. Shows hypothesis text as you speak. Press Enter to confirm, Backspace to delete last line. Auto-stops after silence (unless `-NoAutoStop`).

Requires Azure Speech key and region.

## EXAMPLES

### Example 1: Basic STT
```powershell
$text = Read-AzureSpeech
```

### Example 2: Japanese
```powershell
$text = Read-AzureSpeech -Language ja-JP
```

### Example 3: Long recording
```powershell
$text = Read-AzureSpeech -NoAutoStop -InitialTimeoutSeconds 120
```

### Example 4: Detailed results
```powershell
$results = Read-AzureSpeech -PassThru
$results | Format-Table Text, Duration, Timestamp
```

## PARAMETERS

### -Language
Recognition language (e.g., `en-US`, `ja-JP`). Defaults to system culture. Tab completion supported.

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
Seconds to wait for first speech (1-300). Default: 30.

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
Silence seconds to trigger auto-stop (1-60). Default: 3.

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
Disable auto-stop. Recording until Enter or timeout.

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
Return objects with Text, Duration, Timestamp instead of single string.

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

### -Key
Azure subscription key. Priority: parameter > config. Use `Set-AzureSpeechConfig -Key` to persist.

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
Azure region. Priority: parameter > config. Use `Set-AzureSpeechConfig -Region` to persist.

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
With `-Detailed`, returns objects with Text, Offset, Duration properties.

## NOTES
- Real-time streaming recognition (not batch). Free tier: 5h/month.
- No audio input: check microphone with `Get-Microphone` and `Test-Microphone`.
- Error "Azure Speech key not configured": run `Set-AzureSpeechConfig -Key "..." -Region "..."`.

## RELATED LINKS

[Out-AzureSpeech](Out-AzureSpeech.md)

[Set-AzureSpeechConfig](Set-AzureSpeechConfig.md)