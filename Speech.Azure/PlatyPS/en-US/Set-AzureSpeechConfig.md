---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Set-AzureSpeechConfig

## SYNOPSIS
Configure Azure-specific speech settings.

## SYNTAX

```
Set-AzureSpeechConfig [-Voice <String>] [-Pitch <Int32>] [-Key <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Saves Azure Speech settings to `~/.speech/config.json`. See EXAMPLES for initial setup workflow.

## EXAMPLES

### Example 1: Set credentials
```powershell
Set-AzureSpeechConfig -Key "abc123" -Region "japaneast"
```

### Example 2: Set voice (Tab completion)
```powershell
Set-AzureSpeechConfig -Voice "en-US-JennyNeural"
```

### Example 3: Adjust pitch
```powershell
Set-AzureSpeechConfig -Pitch 10
```

## PARAMETERS

### -Voice
Default Azure neural voice. Tab completion lists voices from API.

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

### -Pitch
Default pitch in Hz (-50 to +50).

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Key
Azure Speech subscription key. From Azure Portal > Speech resource > Keys and Endpoint.

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
Azure region (e.g., `eastus`, `japaneast`, `westeurope`).

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

## NOTES
- Key is stored in plaintext in config. Azure free tier (F0): 0.5M chars TTS + 5h STT/month.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-AzureSpeech](Get-AzureSpeech.md)

[Out-AzureSpeech](Out-AzureSpeech.md)