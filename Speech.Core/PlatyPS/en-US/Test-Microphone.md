---
external help file: Speech.Core.dll-Help.xml
Module Name: Speech.Core
online version:
schema: 2.0.0
---

# Test-Microphone

## SYNOPSIS
Test microphone input levels with a visual meter.

## SYNTAX

```
Test-Microphone [-TestDurationSeconds <Int32>] [-Microphone <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Records from the microphone for a specified duration and displays a real-time visual level meter. Reports whether audio was detected with diagnostic information.

Use before speech recognition (Read-*Speech) to verify the microphone works.

## EXAMPLES

### Example 1: Quick test (5 seconds)
```powershell
Test-Microphone
```

### Example 2: Test specific microphone for 10 seconds
```powershell
Test-Microphone -Microphone "Headset Microphone" -TestDurationSeconds 10
```

### Example 3: Troubleshooting flow
```powershell
Get-Microphone                                       # List devices
Test-Microphone -Microphone "Headset Microphone"     # Verify input
Set-SpeechConfig -Microphone "Headset Microphone"    # Save as default
```

## PARAMETERS

### -TestDurationSeconds
Test duration in seconds (1-60). Default: 5.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 5
Accept pipeline input: False
Accept wildcard characters: False
```

### -Microphone
Microphone device name. Tab completion supported. Uses config default if not specified.

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

### System.Object

## NOTES
- Level > 30: working properly. Level 10-30: volume low. Level < 10: check connection.

## RELATED LINKS

[Get-Microphone](Get-Microphone.md)

[Set-SpeechConfig](Set-SpeechConfig.md)