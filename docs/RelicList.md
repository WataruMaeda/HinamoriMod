# 雛森桃MOD レリック一覧

> Slay the Spire 2 / BLEACH 雛森桃MOD
>
> **レリックの仕様・実装状況はこのファイルで管理する。** カードは `docs/CardList.md` で管理する。
> 表示文言は `HinamoriMod/localization/jpn/relics.json` を優先して確認し、C#実装との差異は明記する。

## 既存レリック（1種類）

| 名前 | クラス名 | レアリティ | 日本語テキストに記載された効果 | C#実装状況 |
| --- | --- | --- | --- | --- |
| 鬼道の心得 | `KidoFundamentals` | Starter | ①戦闘開始時に詠唱を3獲得 ②ランダムなレリックを1個獲得 ③ボスを倒すと追加でレリックを1個ドロップ | ①実装済・動作未確認 ②**取得処理が見当たらない（未実装の可能性）** ③ボス報酬に`RelicReward`を追加する処理あり・動作未確認 |

### 鬼道の心得：確認したソース

- 日本語：`HinamoriMod/localization/jpn/relics.json` の `HINAMORIMOD-KIDO_FUNDAMENTALS.description`。
- C#：`HinamoriModCode/Relics/KidoFundamentals.cs`。
- 詠唱量：`CanonicalVars` の `Chant = 3m`。
- 戦闘開始時：`AfterRoomEntered` で戦闘部屋の場合に `ChantPower` を付与。
- ボス報酬：`TryModifyRewards` で `RoomType.Boss` の場合に `new RelicReward(player)` を追加。
- ランダムレリック1個：`HasReceivedNeowRelic` プロパティはあるが、確認したクラス内にはレリックを実際に付与する処理がない。ほかのファイルからの実装の有無は要確認。
- 英語テキスト：`HinamoriMod/localization/eng/relics.json` は戦闘開始時の詠唱獲得のみで、ランダムレリック取得・ボス報酬追加が記載されていない。翻訳の整合性を修正する必要あり。

## 新規レリック案

現時点で**追加採用が確定したレリックはない**。候補はユーザーと効果・名前・レアリティを確定してから追記する。

## 今後の管理ルール

1. レリック名、レアリティ、通常効果、実装クラス、実装状況をここで管理する。
2. ローカライズの表示テキストとC#の実装が異なる場合は、双方を明記して確認する。
3. 未検証コードは「動作確認済み」としない。
4. 安定版 `Ver.1.0.0` は無断で変更しない。
