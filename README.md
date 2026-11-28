# HinamoriMod

BLEACHの雛森桃をモチーフにした **Slay the Spire 2** 用MODの開発リポジトリです。

## AI・新しいチャットでの作業開始

**最初に [AGENTS.md](AGENTS.md) を読んでください。** 作業ブランチ、既存仕様の保護、資料の参照順、GitHubの変更・検証・報告手順を記載しています。

新しいチャットでは、次のように依頼できます。

> GitHubの WataruMaeda/HinamoriMod の AGENTS.md を最初に読み、GameReference と作業ブランチの実コードを確認してから開発を進めてください。

## ゲーム本体API・処理の解析資料

- **[GameReference/README.md](GameReference/README.md)** — API、カード使用・コスト計算、パワーなどの資料一覧
- **[GameReference/GameVersion.md](GameReference/GameVersion.md)** — 解析対象のSteamビルド・DLL SHA-256
- [GameReference/API/](GameReference/API/) — 主要クラス・メソッドのシグネチャ
- [GameReference/Mechanics/](GameReference/Mechanics/) — 処理フロー
- [GameReference/Powers/](GameReference/Powers/) — 本体パワーの個別解析

資料は特定バージョンのDLLを静的解析したものです。ゲーム更新後はバージョン照合が必要です。

## 主なコード

- `HinamoriModCode/Cards/` — 雛森カードと共通の詠唱処理
- `HinamoriModCode/Powers/` — 独自パワー
- `HinamoriModCode/Relics/` — 独自レリック
- `HinamoriModCode/Patches/` — Harmonyパッチ
- `HinamoriModCode/MainFile.cs` — MOD初期化
- `HinamoriMod/localization/` — ローカライズ

## ブランチについて

本書作成時の継続開発ブランチは `feature/ongoing-fixes` です。基準版 `Ver.1.0.0` を変更・マージする際は、必ずユーザーの明示的な指示を確認してください。

## 注意

ゲーム本体のDLL、逆コンパイルしたソース一式、ゲームアセットは本リポジトリに保存せず、必要なAPI情報と独自の解析要約のみを `GameReference` に管理します。
