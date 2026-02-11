---
external help file: Speech.Azure.dll-Help.xml
Module Name: Speech.Azure
online version:
schema: 2.0.0
---

# Out-AzureSpeech

## SYNOPSIS
Speak text aloud using Azure Speech Services (TTS).

## SYNTAX

```
Out-AzureSpeech [-Text] <String> [-Language <String>] [-Voice <String>] [-Rate <Double>] [-Volume <Int32>]
 [-Pitch <Int32>] [-OutputDevice <String>] [-Key <String>] [-Region <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Synthesizes speech from text using Azure Cognitive Services and plays it through the audio device. Supports SSML prosody (rate, volume, pitch) and 400+ neural voices. Voice resolution priority: `-Voice` > config voice > `-Language` > config language > default (`en-US-JennyNeural`). Detects language mismatch between text and voice.

Requires Azure Speech key and region. Run `Set-AzureSpeechConfig -Key "..." -Region "..."` first.

## EXAMPLES

### Example 1: Simple TTS
```powershell
Out-AzureSpeech "Hello, how are you?"
```

### Example 2: Japanese
```powershell
Out-AzureSpeech "こんにちは、世界" -Language ja-JP
```

### Example 3: Specific voice with rate
```powershell
Out-AzureSpeech "Good morning" -Voice "en-US-GuyNeural" -Rate 0.8
```

### Example 4: Pipeline
```powershell
"Line 1", "Line 2" | Out-AzureSpeech
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

### -Language
Language code (e.g., `en-US`, `ja-JP`). Auto-selects a voice for the language. Tab completion supported.

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

### -Voice
Azure neural voice name (e.g., `ja-JP-NanamiNeural`). Use `Get-AzureSpeech` to list. Tab completion supported.

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
Speech speed (0.5-2.0). 1.0 = normal.

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

### -Volume
Volume percentage (0-100).

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

### -Pitch
Voice pitch in Hz (-50 to +50). 0 = normal.

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
Azure region (e.g., `eastus`, `japaneast`). Priority: parameter > config.

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
- Free tier (F0): 0.5M characters/month neural TTS.
- Error "Azure Speech key not configured": run `Set-AzureSpeechConfig -Key "..." -Region "..."`.
- Error "Voice not found for language": check available voices with `Get-AzureSpeech -Locale <lang>`.

## RELATED LINKS

[Get-AzureSpeech](Get-AzureSpeech.md)

[Read-AzureSpeech](Read-AzureSpeech.md)

[Set-AzureSpeechConfig](Set-AzureSpeechConfig.md)