---
external help file: Speech.Google.dll-Help.xml
Module Name: Speech.Google
online version:
schema: 2.0.0
---

# Set-GoogleSpeechConfig

## SYNOPSIS
Configure Google Cloud-specific speech settings.

## SYNTAX

```
Set-GoogleSpeechConfig [-Voice <String>] [-Credential <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Saves Google Cloud settings to `~/.speech/config.json`. See EXAMPLES for initial setup workflow.

## EXAMPLES

### Example 1: Set credential
```powershell
Set-GoogleSpeechConfig -Credential "C:\keys\my-project-sa.json"
```

### Example 2: Set voice
```powershell
Set-GoogleSpeechConfig -Voice "en-US-Wavenet-D"
```

## PARAMETERS

### -Voice
Default Google TTS voice. Tab completion supported (API-backed).

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

### -Credential
Path to service account JSON file. Must exist. **Do not commit to git.**

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
- Credential path stored; file not embedded. Ensure .gitignore excludes credential JSON.

## RELATED LINKS

[Get-SpeechConfig](../../../Speech.Core/PlatyPS/en-US/Get-SpeechConfig.md)

[Get-GoogleSpeech](Get-GoogleSpeech.md)

[Out-GoogleSpeech](Out-GoogleSpeech.md)