# Voice モジュール Cmdlet 整理・実装 作業手順書

## 概要

音声認識 Cmdlet の命名統一と Azure STT の実装を行う。

## 作業項目

### 1. Cmdlet 改名

| 現在 | 変更後 |
|------|--------|
| `Wait-WindowsVoiceInput` | `Read-WindowsVoice` |
| `Wait-AzureVoiceInput` | `Read-AzureVoice` |

### 2. Read-AzureVoice 実装

- Enter 押下で終了
- 複数発話は結合して1文字列で返す
- Azure Speech SDK を使用（REST API では連続認識不可）

### 3. psd1 更新

公開 Cmdlet を 11 個に整理（Wait-AzureVoiceInput 除外 → Read-AzureVoice 追加）

### 4. README 更新

新しい Cmdlet 名と仕様に合わせて更新

## 品質基準

- ビルドが通ること (`dotnet build -c Release`)
- 改名後の Cmdlet がモジュールからエクスポートされること
- Read-AzureVoice が Enter 押下で終了し、認識テキストを返すこと

## リスク

| リスク | 対策 |
|--------|------|
| Azure Speech SDK の追加で DLL サイズ増大 | Costura.Fody で単一 DLL 維持 |
| SDK の非同期 API と PowerShell の相性 | 同期的にラップして提供 |

## コミットポリシー

- テスト通過 + Yoshifumi のレビュー承認後にコミット

## 進捗更新ルール

- 作業進捗は work_progress.md に即時反映

## 学習事項

- (作業中に得た知見をここに追記)