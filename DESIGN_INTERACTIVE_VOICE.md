# VoiceHub - Interactive Voice Interface Design

## 概要

PowerShell用の音声対話インターフェース。音声出力（TTS）と音声認識（STT）を統合し、自然な会話型インタラクションを実現。

### 設計方針

- **シンプル**: セッション管理なし、グローバルキュー
- **非同期**: TTS は常にバックグラウンド実行
- **自動キューイング**: 連続音声を順次再生
- **Barge-in 対応**: 再生中の割り込み検出

---

## 実装状況

### Phase 1: Windows Speech ✅
- `VoiceState` - グローバルキュー管理
- `WindowsVoiceRequest` - Windows TTS 実装
- `Out-WindowsVoice` - 音声出力
- `Wait-WindowsInput` - 音声認識（Barge-in 対応）
- `Get-WindowsVoice` - 音声一覧

### Phase 2: Barge-in サポート ✅
- `Get-VoiceQueueState` - キュー状態監視
- `Clear-VoiceQueue` - キューのクリア

### コード統計
- **合計: 380行**
  - Core: 137行（VoiceState, WindowsVoiceRequest）
  - Cmdlets: 243行（5 cmdlets）

### リファクタリング
- VoiceOutputRequest 抽象クラスを削除（YAGNI 原則）
- 101行削減（26%）
- より直接的でシンプルなコード

---

## アーキテクチャ

### VoiceState（静的クラス）

グローバルな音声出力キュー管理。

```csharp
public static class VoiceState
{
    private static ConcurrentQueue<WindowsVoiceRequest> _outputQueue;
    
    public static void EnqueueOutput(WindowsVoiceRequest request);
    public static void ClearQueue();
    
    public static bool IsQueueEmpty { get; }
    public static int QueueSize { get; }
    public static bool IsPlaying { get; }
}
```

**キュー処理:**
- バックグラウンドタスクで順次再生
- CancellationToken で中断可能
- Barge-in 時にキュークリア

### WindowsVoiceRequest

```csharp
public class WindowsVoiceRequest
{
    public string Text { get; set; }
    public string? Voice { get; set; }
    public int Rate { get; set; }
    public int Volume { get; set; }
    
    public Task PlayAsync(CancellationToken cancellationToken);
}
```

**実装:**
- SpeechSynthesizer でWAV生成
- NAudio で再生
- キャンセル可能な待機ループ

---

## Cmdlets

### Out-WindowsVoice
音声出力（自動キューイング）

```powershell
Out-WindowsVoice "Hello" [-Voice <string>] [-Rate <int>] [-Volume <int>] [-Wait]
```

### Wait-WindowsInput
音声認識（Barge-in 対応）

```powershell
Wait-WindowsInput [-TimeoutSeconds <int>] [-Language <string>] [-Confidence <double>] [-CancelOnSpeech]
```

**Barge-in 判定:**
- 発話時間 < 0.8秒 → 相づち（無視）
- 相づちパターンマッチ → 無視
- それ以外 → Barge-in（キュークリア）

### Get-VoiceQueueState
キュー状態取得

```powershell
Get-VoiceQueueState
# → QueueSize, IsPlaying, IsQueueEmpty
```

### Clear-VoiceQueue
キューのクリア

```powershell
Clear-VoiceQueue [-Confirm]
# → WasPlaying, QueueSizeCleared
```

### Get-WindowsVoice
利用可能な音声一覧

```powershell
Get-WindowsVoice
```

---

## 使用例

### 基本的な会話ループ

```powershell
Out-WindowsVoice "こんにちは。お名前は？"
$name = Wait-WindowsInput -TimeoutSeconds 30

Out-WindowsVoice "こんにちは、${name}さん"
```

### Barge-in 対応

```powershell
Out-WindowsVoice "長いメッセージです..."
Out-WindowsVoice "これも再生されます..."
Out-WindowsVoice "これも..."

# 再生中に話しかけるとキューがクリアされる
$input = Wait-WindowsInput -CancelOnSpeech
```

---

## 未実装機能

### Phase 3: Azure Speech Service
- Azure TTS/STT サポート
- 高品質な音声
- 必要時に実装

### Phase 4: OpenAI
- OpenAI TTS サポート
- 必要時に実装

---

## 技術メモ

### 相づち判定ロジック
- 発話時間 < 0.8秒
- パターン: "うん", "ええ", "はい", "へー", "ほー", "なるほど", "そうですか"

### エラーハンドリング
- 最小限のtry-catch
- エラー時は継続（次の音声へ）

### パフォーマンス
- Cmdlet は即座に完了（非同期）
- UIブロックなし
- キューイングで効率的に管理

---

## 開発メモ

### インポート方法
```powershell
# 開発中
Import-Module "C:\MyProj\VoiceHub\VoiceHub\bin\Debug\net9.0\VoiceHub.dll" -Force

# デプロイ後
Import-Module VoiceHub
```

### テストスクリプト
```powershell
C:\MyProj\VoiceHub\Test-Phase2-Integration.ps1
```
