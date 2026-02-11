---
external help file: Speech.Core.dll-Help.xml
Module Name: Speech.Core
online version:
schema: 2.0.0
---

# Get-Microphone

## SYNOPSIS
List available microphone input devices.

## SYNTAX

```
Get-Microphone [-All] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Enumerates audio input (microphone) devices using NAudio. Returns device index, name, and channel count. Use the device name with `-Microphone` on speech recognition cmdlets or `Set-SpeechConfig -Microphone`.

## EXAMPLES

### Example 1: List all microphones
```powershell
Get-Microphone
```

### Example 2: Set preferred microphone from list
```powershell
Get-Microphone | Select-Object -First 1 | ForEach-Object { Set-SpeechConfig -Microphone $_.Name }
```

## PARAMETERS

### -All
Reserved for future use. All devices are always listed.

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

### Speech.Core.MicrophoneInfo

## NOTES
- Windows only. Device names may be truncated (32 chars max).
- Device index 0 is typically the system default microphone.

## RELATED LINKS

[Test-Microphone](Test-Microphone.md)

[Set-SpeechConfig](Set-SpeechConfig.md)