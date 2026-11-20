# 雛森桃MOD 専用ポーション一覧

> 2026-11-20 ユーザー承認済みの6種類。実装ブランチ: `feature/card-implementation`。
> **6種類ともBaseLibのCustomPotionModelとして効果処理・プール指定を追加済み。ただしビルド・ゲーム内使用は未検証で、画像未追加。**

| 名前 | クラス（仕様定数） | レアリティ | 確定効果 | 状態 |
| --- | --- | --- | --- | --- |
| 霊圧の霊薬 | `SpiritualElixir` | Common | 詠唱+5 | C#効果処理あり・未検証 |
| 飛梅の火粉 | `TobiumeSparks` | Common | 火種+15 | C#効果処理あり・未検証 |
| 灼熱の霊薬 | `ScorchingElixir` | Uncommon | 敵1体に火傷10 | C#効果処理あり・未検証 |
| 鬼道の結晶 | `KidoCrystal` | Uncommon | このターン、破道・縛道の詠唱コスト0 | C#効果処理あり・未検証 |
| 飛梅の霊髄 | `TobiumeEssence` | Rare | 火種+30、敵全体に火傷5 | C#効果処理あり・未検証 |
| 完全詠唱の秘薬 | `PerfectIncantationElixir` | Rare | 詠唱+10、次の破道・縛道のダメージ・ブロック2倍 | C#効果処理あり・未検証 |

## 実装残タスク

- [x] 6種類の効果・名前・レアリティを確定し、C#のポーションクラスを作成
- [x] BaseLib `CustomPotionModel` を継承した6種類のクラスを作成（外部のBaseLib実装・MOD作成チュートリアルを参照。ローカルビルド未確認）
- [x] 6種類に `[Pool(typeof(HinamoriModPotionPool))]` を指定（実際の抽選・ドロップは未検証）
- [x] 霊圧の霊薬：使用時 `ChantPower` を5増加
- [x] 飛梅の火粉：使用時 `EmberCmd.GainEmber(15m, player)` を実行
- [x] 灼熱の霊薬：敵単体選択後、`BurnPower` を10付与
- [x] 鬼道の結晶：使用ターンだけ破道・縛道の詠唱コストを0にする。コスト表示と実消費を一致させ、黒棺の例外・リプレイを検証
- [x] 飛梅の霊髄：火種30獲得後、敵全体に火傷5。火種獲得に伴う炎の循環の発動を検証
- [x] 完全詠唱の秘薬：詠唱10獲得、次の破道・縛道にダメージ・ブロック2倍を適用。既存の完全詠唱パワーとの重複・消費順序を確認
- [x] `HinamoriMod/localization/jpn/potions.json` と `eng/potions.json` を追加し、鬼道の結晶のパワー説明も日英登録（ロード確認は未実施）
- [ ] 6種類の画像・アウトライン等を作成して登録する
- [ ] ビルド・実ゲームでの使用・ターゲット選択・ドロップ・セーブロード・マルチプレイを検証する

## 実装上の注意

`HinamoriModCode/Character/HinamoriModPotionPool.cs` は既存のプールクラスだが、現状ではポーション個別の登録処理は確認できない。カード・レリックと同様の自動登録が使えるかも検証する。

6種類は `CustomPotionModel` を継承し、`OnUse` と `PotionRarity`・`PotionUsage`・`TargetType` を実装した。**ビルド・実ゲーム検証前なので使用可能とは断言できない。**

## 実装後の追加確認事項

- [ ] **画像が未追加**。ポーションの通常画像・アウトラインを配置し、BaseLibの `CustomPackedImagePath` / `CustomPackedOutlinePath` で参照する。現状はデフォルト画像パスで見つからない可能性がある。
- [ ] **鬼道の結晶**：`PotionFreeKidoChantPower` を追加し、`HinamoriModCard.EffectiveChantCost` と `SpendChant` に接続した。使用ターン終了時に解除されること、黒棺に影響しないこと、既存軽減と重複しても不具合がないことを確認。
- [ ] **完全詠唱の秘薬**：既存の `FullIncantationPower` に2スタック相当を適用。既存効果との重複、ターンをまたぐ持続、対象カードの初回使用で解除されることを確認。
- [ ] **対象選択**：`ScorchingElixir` の `TargetType.AnyEnemy` が実際に敵単体選択として動くことを確認。
- [ ] **パワーと表示**：ポーション用の新規パワーのアイコンや動的説明、日英テキストの読み込みを確認。
- [ ] **ゲーム互換性**：参照している外部BaseLibコードと実際の導入バージョンのAPI差分を確認してビルドする。
