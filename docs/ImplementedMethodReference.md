# 雛森桃MOD：実装済みカードのメソッド・処理リファレンス

> **調査基準**：`feature/card-implementation` のC#実装を直接確認した内容。カードの説明文だけからAPIを推測していない。  
> **重要**：本資料は今回直接確認できた **12カード＋共通基底クラス** を対象とする初版。リポジトリ内の全カードの網羅確認はまだ完了していない。未確認のカードについて「メソッドが存在しない」とは判断しない。  
> **検証状況**：ソース上の呼び出しを確認済み。ビルドおよびゲーム内挙動は未検証。

## 1. 確認したカードと主な処理

| カード | C#ファイル（`HinamoriModCode/Cards/` 以下） | 使用する主なメソッド・プロパティ | ソース上で確認した用途 |
|---|---|---|---|
| 白雷 | `WhiteLightning.cs` | `DamageCmd.Attack`, `CardPileCmd.Draw` | 単体攻撃→ドロー |
| 蒼火墜 | `BlueFirefall.cs` | `SpendChant`, `DamageCmd.Attack`, `EmberCmd.GainEmber` | 詠唱消費→全体攻撃→火種獲得 |
| 赤煙遁 | `RedSmokeEscape.cs` | `CreatureCmd.GainBlock`, `CardPileCmd.Draw` | ブロック獲得→ドロー |
| 霊圧集中 | `SpiritualPressureFocus.cs` | `PowerCmd.Apply<ChantPower>`, `CardPileCmd.Draw` | 詠唱獲得→ドロー |
| 霊圧解放 | `SpiritualPressureRelease.cs` | `CardSelectCmd.FromHand`, `CardCmd.Exhaust`, `PowerCmd.Apply<ChantPower>` | 手札から選択／ランダム廃棄→詠唱獲得 |
| 鬼道の才 | `KidoProdigy.cs` | `CardSelectCmd.FromHandForUpgrade`, `CardCmd.Upgrade` | 手札のカードを戦闘中強化 |
| 掴趾追雀 | `TracingSparrow.cs` | `CardFactory.GetDistinctForCombat`, `CardSelectCmd.FromChooseACardScreen`, `CardPileCmd.AddGeneratedCardToCombat` | 候補生成→1枚選択→無料化・リプレイ付与→手札へ |
| 鎖条鎖縛 | `ChainBinding.cs` | `PileType.Exhaust.GetPile`, `CardSelectCmd.FromChooseACardScreen`, `CardPileCmd.Add` | 廃棄札から1枚選択して手札へ |
| 炎の気配 | `FlamesPresence.cs` | `EmberCmd.GainEmber`, `RemoveKeyword` | 火種獲得、強化後に廃棄キーワード除去 |
| 逆境の炎 | `AdversityFlame.cs` | `PileType.Hand.GetPile`, `CardCmd.Exhaust`, `EmberCmd.GainEmber` | 状態異常（強化後は呪いも）を廃棄し、成功枚数に応じて火種獲得 |
| 黒棺 | `BlackCoffin.cs` | `GetPower<ChantPower>`, `PowerCmd.ModifyAmount`, `DamageCmd.Attack` | 初回だけ詠唱を全消費し、リプレイで消費量を再利用 |
| 霊力変換 | `SpiritualEnergyConversion.cs` | `SpendChant`, `PlayerCmd.GainEnergy` | 詠唱消費→エナジー獲得 |

## 2. 処理別メソッド早見表

| やりたいこと | 既存のメソッド・記法 | 実装例 | 注意点 |
|---|---|---|---|
| 手札の一覧取得 | `PileType.Hand.GetPile(Owner).Cards` | 霊圧解放、逆境の炎 | 廃棄等でリストが変わる場合は `.ToList()` で固定 |
| 廃棄札の一覧取得 | `PileType.Exhaust.GetPile(Owner).Cards` | 鎖条鎖縛、逆境の炎 | 廃棄成功判定に使う場合はカードの移動結果を確認 |
| 指定カードを廃棄 | `await CardCmd.Exhaust(choiceContext, card)` | 霊圧解放、逆境の炎 | 廃棄不可カードの挙動は未検証 |
| 手札から選択 | `await CardSelectCmd.FromHand(...)` | 霊圧解放 | `CardSelectorPrefs` で廃棄選択画面を指定 |
| 手札から強化対象を選択 | `await CardSelectCmd.FromHandForUpgrade(choiceContext, Owner, this)` | 鬼道の才 | 強化可能なカードの選択に使用 |
| 任意の候補リストから選択 | `await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner, canSkip: false)` | 掴趾追雀、鎖条鎖縛 | `canSkip: false` で選択スキップ不可 |
| カードを強化 | `CardCmd.Upgrade(card)` | 鬼道の才 | `card.IsUpgradable` で対象を絞る例あり |
| 火種を獲得 | `await EmberCmd.GainEmber(amount, Owner)` | 蒼火墜、炎の気配、逆境の炎 | 火種獲得に連動する効果があるため呼び出し回数にも注意 |
| 詠唱を獲得 | `await PowerCmd.Apply<ChantPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this)` | 霊圧集中、霊圧解放 | 使用者と対象の引数を区別 |
| 詠唱を消費 | `await SpendChant(choiceContext, cardPlay)` | 蒼火墜、霊力変換、鎖条鎖縛 | `HinamoriModCard` の共通処理。失敗時は `return` |
| 詠唱パワーを取得 | `Owner.Creature.GetPower<ChantPower>()` | 黒棺、共通基底 | パワーがない場合は `null` |
| パワーの量を増減 | `await PowerCmd.ModifyAmount(...)` | 黒棺、共通基底 | 黒棺は負数で詠唱を消費 |
| エナジーを獲得 | `await PlayerCmd.GainEnergy(amount, Owner)` | 霊力変換 | `EnergyVar` の値を渡す |
| カードを引く | `await CardPileCmd.Draw(choiceContext, amount, Owner)` | 白雷、赤煙遁、霊圧集中 | `CardsVar` と組み合わせる |
| ブロックを得る | `await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay)` | 赤煙遁 | `BlockVar` を利用 |
| 敵1体に攻撃 | `await DamageCmd.Attack(amount).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext)` | 白雷、黒棺 | 対象が必要。ヒット演出は `WithHitFx` |
| 敵全体に攻撃 | `...TargetingAllOpponents(CombatState).Execute(choiceContext)` | 蒼火墜 | `DamageCmd.Attack` のターゲット設定を変更 |
| カード候補を生成 | `CardFactory.GetDistinctForCombat(Owner, candidates, 3, Owner.RunState.Rng.CombatCardGeneration)` | 掴趾追雀 | 戦闘用のカードを重複なしで最大3枚 |
| 生成カードを手札へ | `await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner)` | 掴趾追雀 | 新規生成したカード用 |
| 既存カードを手札へ | `await CardPileCmd.Add(card, PileType.Hand)` | 鎖条鎖縛 | 廃棄札からの回収で使用 |
| このターンのEN無料化 | `card.SetToFreeThisTurn()` | 掴趾追雀 | 一時的な効果 |
| このターンの詠唱無料化 | `momoCard.SetChantFreeThisTurn()` | 掴趾追雀、共通基底 | 雛森専用カードの独自メソッド |
| リプレイ付与 | `card.BaseReplayCount += count` | 掴趾追雀 | この実装では戦闘中持続 |
| 廃棄キーワード除去 | `card.RemoveKeyword(CardKeyword.Exhaust)` | 炎の気配、鎖条鎖縛 | カード自体の廃棄属性を除く |
| 保留キーワード付与 | `card.AddKeyword(CardKeyword.Retain)` | 鎖条鎖縛 | 回収カードを保留させる |
| ENコストの強化 | `EnergyCost.UpgradeBy(-1)` | 鎖条鎖縛 | 1EN→0ENの例 |
| 数値の強化 | `DynamicVars.Damage.UpgradeValueBy(...)` 等 | 白雷、蒼火墜 | 説明文と実効果に使う値を更新 |

## 3. よく使う実装パターン

### 3-1. 手札の対象カードをまとめて廃棄する

出典：`AdversityFlame.cs`、`SpiritualPressureRelease.cs`

```csharp
// 廃棄で手札の内容が変わるので、対象一覧を先に固定する。
var targets = PileType.Hand.GetPile(Owner).Cards
    .Where(card => card.Type == CardType.Status)
    .ToList();

foreach (CardModel card in targets)
{
    // ゲーム本体の廃棄コマンドを使う。
    await CardCmd.Exhaust(choiceContext, card);
}
```

※廃棄できた枚数の判定が必要な場合、逆境の炎では廃棄後の `PileType.Exhaust` を確認している。ただし廃棄不可など特殊なカードでの動作は未確認。

### 3-2. 詠唱コストを消費してから効果を発動する

出典：`SpiritualEnergyConversion.cs`、`HinamoriModCard.cs`

```csharp
// 詠唱が足りなければ、以降の効果を発動しない。
if (!await SpendChant(choiceContext, cardPlay))
    return;

// 詠唱を消費できた場合だけエナジーを獲得する。
await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
```

共通基底の `SpendChant` は、リプレイ時の再消費回避、詠唱無料化、各種詠唱コスト軽減などを扱う。カード側で独自に詠唱を減らす前にこの処理を確認する。

### 3-3. 攻撃とドローを組み合わせる

出典：`WhiteLightning.cs`

```csharp
await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
    .FromCard(this, cardPlay)
    .Targeting(cardPlay.Target)
    .WithHitFx("vfx/vfx_attack_slash")
    .Execute(choiceContext);

await CardPileCmd.Draw(
    choiceContext, DynamicVars.Cards.BaseValue, Owner);
```

## 4. 共通基底クラス `HinamoriModCard` の重要な仕組み

- `ChantCost`：追加の詠唱コスト。既定値 `-1` は詠唱コストを持たない扱い。
- `HinamoriTags` と `HasHinamoriTag`：破道・鬼道などの独自タグ判定。
- `EffectiveChantCost`：無料化・軽減を考慮した現在の詠唱コストを計算。
- `SpendChant`：詠唱を実際に消費。リプレイ時は再消費しない。
- `SetChantFreeThisTurn` / `ClearChantFreeThisTurn`：一時的な詠唱無料化の設定・解除。
- `AfterSideTurnEnd`：所有者側のターン終了時に一時無料化を解除。
- `IsPlayable`：通常の使用可否に加え、必要な詠唱が足りるかを確認。
- `PortraitPath` / `CustomPortraitPath` / `BetaPortraitPath`：カード画像のパスを生成。

## 5. 今後の運用

1. 新しいカードを作る前に本書を確認し、既存のコマンド・メソッドを再利用する。
2. 本書にない処理は、**既存C#の追加調査**を先に行う。`docs/参考_Base` はカード名・Localization等の参考資料であり、本体C#ソースではない。
3. 本体コードがどうしても必要なら、確認したい標準カード名・処理を具体的に記録する。
4. 新規カード実装時は本書の使用例・カード対応表を追記し、`docs/CardList.md` を **「実装済・未確認」** にする。
5. 変更は `docs/CHANGELOG.md` に記録し、**日本語のコミットメッセージと日本語のコードコメント**を使用する。
6. ビルド・ゲーム内テストの結果を確認するまで、動作確認済みと表記しない。

## 6. 未確認・今後の調査対象

- 今回の対象外だった残りのC#カードファイルのメソッド使用状況（**全カード網羅は未完了**）。
- 状態異常・呪いの分類や廃棄不可カードの正確な動作。
- `CardCmd.Exhaust` の特殊カードに対する動作、廃棄後の効果発動順序。
- `EmberCmd.GainEmber` の1回の呼び出しと複数回の呼び出しで、関連パワーがどう発火するか。

## 7. カード一覧との突合（設計上の全カード）

以下は `docs/CardList.md` に掲載されているカードの重複を除いた一覧。**「C#確認済」以外はC#ファイル未調査であり、未実装という意味ではない。** 一覧の「実装状況」は設計表の記載をそのまま転記したもので、ソースの存在を保証しない。

| カード | クラス名（設計表） | 設計表の実装状況 | C#ソースの確認 |
|---|---|---|---|
| 斬撃 | `SlashMomo` | 実装済 | 未調査 |
| 瞬歩 | `ShunpoMomo` | 実装済 | 未調査 |
| 精神統一 | `MeditationMomo` | 実装済 | 未調査 |
| 赤火砲 | `RedFrameCannonMomo` | 実装済 | 未調査 |
| 衝 | `ThrustMomo` | 実装済 | 未調査 |
| 白雷 | `WhiteLightingMomo` | 実装済 | 未調査 |
| 綴雷電 | `BoundLightningMomo` | 実装済 | 未調査 |
| 伏火 | `CrawlingFlameMomo` | 実装済 | 未調査 |
| 黄火閃 | `YellowFireFlashMomo` | 実装済 | 未調査 |
| 蒼火墜 | `BlueFirefallMomo` | 実装済 | 確認済（本書に記載） |
| 廃炎 | `AbolishingFlamesMomo` | 実装済 | 未調査 |
| 闐嵐 | `RagingStormMomo` | 実装済 | 未調査 |
| 雷吼炮 | `ThunderRoarCannonMomo` | 実装済 | 未調査 |
| 双蓮蒼火墜 | `TwinLotusBlueFirefallMomo` | 実装済 | 未調査 |
| 黒棺 | `BlackCoffinMomo` | 実装済 | 確認済（本書に記載） |
| 飛竜撃賊震天雷炮 | `FlyingDragonStrikingHeavenShakingThunderCannonMomo` | 実装済 | 未調査 |
| 塞 | `RestrainMomo` | 実装済 | 未調査 |
| 這縄 | `CrawlingRopeMomo` | 実装済 | 未調査 |
| 斥 | `RepulseMomo` | 実装済 | 未調査 |
| 崩輪 | `DisintegratingCircleMomo` | 実装済 | 未調査 |
| 赤煙遁 | `RedSmokeEscapeMomo` | 実装済 | 確認済（本書に記載） |
| 曲光 | `BendingLightMomo` | 実装済 | 未調査 |
| 嘴突三閃 | `TripleBeakStrikeMomo` | 実装済 | 未調査 |
| 吊星 | `HangingStarMomo` | 実装済 | 未調査 |
| 円閘扇 | `RoundLockFanMomo` | 実装済 | 未調査 |
| 掴趾追雀 | `TracingSparrowMomo` | 実装済 | 確認済（本書に記載） |
| 六杖光牢 | `SixRodLightPrisonMomo` | 実装済 | 未調査 |
| 百歩欄干 | `HundredStepRailingsMomo` | 実装済 | 未調査 |
| 鎖条鎖縛 | `ChainBindingMomo` | 実装済 | 確認済（本書に記載） |
| 倒山晶 | `InvertedMountainCrystalMomo` | 実装済 | 未調査 |
| 断空 | `SplittingVoidMomo` | 実装済 | 未調査 |
| 詠唱準備 | `ChantPreparationMomo` | 実装済 | 未調査 |
| 霊圧集中 | `SpiritualPressureFocusMomo` | 実装済 | 確認済（本書に記載） |
| 鬼道の修練 | `KidoTrainingMomo` | 実装済 | 未調査 |
| 完全詠唱 | `FullIncantationMomo` | 未確認 | 未調査 |
| 霊圧解放 | `SpiritualPressureReleaseMomo` | 未確認 | 確認済（本書に記載） |
| 詠唱破棄 | `IncantationAbandonmentMomo` | 未確認 | 未調査 |
| 鬼道の才 | `KidoProdigyMomo` | 未確認 | 確認済（本書に記載） |
| 霊力変換 | `SpiritualEnergyConversionMomo` | 未実装 | 確認済（本書に記載） |
| 弾け、飛梅！ | `TobiumeMomo` | 実装済 | 未調査 |
| 飛梅との対話 | `CommunionWithTobiumeMomo` | 未確認 | 未調査 |
| 炎の気配 | `FlamesPresenceMomo` | 実装済 | 確認済（本書に記載） |
| 飛梅の一閃 | `TobiumesSlashMomo` | 未確認 | 未調査 |
| 炎の守り | `FlameGuardMomo` | 未確認 | 未調査 |
| 逆境の炎 | `AdversityFlameMomo` | 実装済・未確認 | 確認済（本書に記載） |
| 藍染隊長の仇 | `AvengeCaptainAizenMomo` | 仕様変更・実装要確認 | 未調査 |
| 飛梅の炎撃 | `TobiumesFlameStrikeMomo` | 未確認 | 未調査 |
| 炎の拘束 | `FlameBindMomo` | 未確認 | 未調査 |
| 燃焼促進 | `AcceleratedCombustionMomo` | 未確認 | 未調査 |
| 飛梅の炎舞 | `TobiumesFlameDanceMomo` | 未確認 | 未調査 |
| 飛梅の火花 | `TobiumesSparkMomo` | 未確認 | 未調査 |
| 炎の爆発 | `FlameExplosionMomo` | 未実装 | 未調査 |
| 炎の吸収 | `FlameAbsorptionMomo` | 未実装 | 未調査 |
| 鬼道の達人 | `KidoMasterMomo` | 実装済 | 未調査 |
| 鬼道の鍛錬 | `KidoDisciplineMomo` | 未確認 | 未調査 |
| 鬼道の真髄 | `EssenceOfKidoMomo` | 未確認 | 未調査 |
| 霊圧制御 | `SpiritualPressureControlMomo` | 未確認 | 未調査 |
| 飛梅と共に | `TogetherWithTobiumeMomo` | 未確認 | 未調査 |
| 炎の循環 | `FlameCycleMomo` | 未確認 | 未調査 |
| 鬼道の探究 | `KidoResearchMomo` | 効果変更・未確認 | 未調査 |
| 霊圧の解放 | `SpiritualPressureUnleashedMomo` | 未確認 | 未調査 |
| 瞬歩の鍛錬 | `ShunpoTrainingMomo` | 未確認 | 未調査 |
| 斬術の鍛錬 | `ZanjutsuTrainingMomo` | 未確認 | 未調査 |
| 白打の鍛錬 | `HakudaTrainingMomo` | 未確認 | 未調査 |
| 受け継ぐ想い | `InheritedResolveMomo` | 未確認 | 未調査 |
| 破道の極致（仮称） | `PinnacleOfHadoMomo` | 未確認 | 未調査 |
| 燃え盛る炎 | `BlazingTobiumeMomo` | 未実装 | 未調査 |
| お前か！！ | `ItsYouMomo` | 未確認 | 未調査 |
| 鬼道の反復 | `KidoRecitationMomo` | 未確認 | 未調査 |


## 8. 全88ソースのメソッド逆引き

全88件のC#ソース本文を調査し、ドット付きメソッド呼び出し **229種類**を抽出した [全ソースメソッド索引](AllSourceMethodIndex.md) を作成した。カードだけでなく、Commands / Powers / Patches / Debug / Character / UI なども含む。抽出は簡易解析であり、単独メソッド呼び出しや定義一覧を網羅するものではない。


## 9. 飛梅の一閃（`TobiumesSlash.cs`）

| メソッド・変数 | 効果・用途 |
|---|---|
| `ArgumentNullException.ThrowIfNull(cardPlay.Target)` | 単体攻撃の対象が指定されていることを確認する |
| `DamageCmd.Attack(...).FromCard(...).Targeting(...).WithHitFx(...).Execute(...)` | カードの攻撃として敵1体に7ダメージを与える |
| `EmberCmd.GainEmber(DynamicVars.Forge.IntValue, Owner)` | 攻撃後に火種を獲得し、飛梅の生成判定も共通処理に任せる |
| `DynamicVars.Forge.UpgradeValueBy(3)` | 強化後の火種獲得量を7から10に増やす |

**実装の参照元**：`Slash.cs` の単体攻撃、`BlueFirefall.cs` の火種獲得と強化処理。詠唱消費なし、1ENのコモンアタック。ソース実装のみ、ビルド・ゲーム内動作未確認。


## 10. 炎の守り（`FlameGuard.cs`）

| メソッド・変数 | 効果・用途 |
|---|---|
| `GainsBlock => true` | ブロック獲得カードとして扱う |
| `new BlockVar(8m, ValueProp.Move)` | ブロック8を表示・処理で共有する |
| `CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay)` | 使用者に8ブロックを付与する |
| `new ForgeVar(5)` | 火種獲得量を表示・処理で共有する |
| `EmberCmd.GainEmber(DynamicVars.Forge.IntValue, Owner)` | ブロック獲得後に火種5を獲得する。飛梅の生成判定も共通処理に任せる |
| `DynamicVars.Forge.UpgradeValueBy(3)` | 強化後の火種獲得量を5から8に増やす。ブロック8は据え置き |

**参照元**：`RedSmokeEscape.cs` のブロック獲得処理、`BlueFirefall.cs`・`TobiumesSlash.cs` の火種獲得と強化処理。詠唱コストなし、1ENのコモンスキル。ビルド・ゲーム内動作未確認。


## 11. 飛梅の炎撃（`TobiumesFlameStrike.cs`）

| メソッド・変数 | 用途 |
|---|---|
| `new DamageVar(12m, ValueProp.Move)` | 攻撃ダメージ12の表示・計算 |
| `new IntVar("Burn", 3)` | 火傷付与数3の表示・処理 |
| `ArgumentNullException.ThrowIfNull(cardPlay.Target)` | 単体攻撃の対象確認 |
| `DamageCmd.Attack(...).FromCard(...).Targeting(...).Execute(...)` | 対象に12ダメージを与える |
| `cardPlay.Target.IsAlive` | 攻撃後に対象が生存している場合のみ火傷を付与する |
| `PowerCmd.Apply<BurnPower>(...)` | 対象へ火傷3を付与する |
| `DynamicVars["Burn"].UpgradeValueBy(2m)` | 強化後の火傷を3から5へ変更する |

**参照元**：`AbolishingFlames.cs`・`CrawlingFlame.cs` の攻撃後火傷付与処理。コモン、2EN、詠唱コストなし。ソース実装済み、ビルド・ゲーム内動作未確認。


## 炎の拘束（`FlameBind.cs`）

| メソッド・変数 | 用途 |
|---|---|
| `GainsBlock => true` | ブロック獲得カードとしてゲーム本体に通知 |
| `new BlockVar(5m, ValueProp.Move)` | 5ブロックの表示・補正・獲得量 |
| `new IntVar("Burn", 2)` | 火傷2の表示と付与数 |
| `ArgumentNullException.ThrowIfNull(cardPlay.Target)` | 火傷付与対象の敵1体を確認 |
| `cardPlay.Target.IsAlive` | 対象が生存している場合のみ火傷付与 |
| `PowerCmd.Apply<BurnPower>(...)` | 敵1体へ火傷を付与 |
| `CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay)` | 自分へブロックを獲得 |
| `DynamicVars["Burn"].UpgradeValueBy(1m)` | 強化後に火傷2→3、ブロック5維持 |

**参考実装**：`Restrain.cs`（敵を対象に取る防御スキル）、`CrawlingFlame.cs`（火傷付与）、`FlameGuard.cs`（ブロック獲得）。ソース実装済み、ビルド・ゲーム内動作未確認。


## 燃焼促進（`AcceleratedCombustion.cs`）

| メソッド・変数 | 用途 |
|---|---|
| `CanonicalKeywords => [CardKeyword.Exhaust]` | 使用後に廃棄。強化後も維持 |
| `ArgumentNullException.ThrowIfNull(cardPlay.Target)` | 敵1体を対象にする |
| `cardPlay.Target.GetPower<BurnPower>()` | 対象の現在の火傷パワーを取得 |
| `burn.Amount` | 既存火傷スタックを参照。0以下なら何もしない |
| `PowerCmd.Apply<BurnPower>(...)` | 現在値と同じ火傷を追加してスタックを2倍にする |
| `EnergyCost.UpgradeBy(-1)` | 強化後のエナジーコストを2→1に変更 |

**参照元**：`BlackCoffin.cs`（`GetPower<T>()` と `Amount`）、`FlameBind.cs`（`PowerCmd.Apply<BurnPower>`）、`FlamesPresence.cs`・`Tobiume.cs`（廃棄キーワード・エナジーコスト強化）。ソース実装済み、ビルド・ゲーム内動作未確認。


## 飛梅の炎舞（`TobiumesFlameDance.cs`）

| メソッド・変数 | 用途 |
|---|---|
| `new DamageVar(6m, ValueProp.Move)` | 攻撃ダメージを表示・計算 |
| `ArgumentNullException.ThrowIfNull(cardPlay.Target)` | 敵1体の対象指定を確認 |
| `cardPlay.Target.GetPower<BurnPower>()?.Amount` | 攻撃前の対象火傷スタック数を取得。火傷なしは0 |
| `Math.Max(0, ...)` | 負の火傷量を火種獲得に使用しない |
| `DamageCmd.Attack(...).FromCard(...).Targeting(...).Execute(...)` | 対象へ6（強化後9）ダメージ |
| `EmberCmd.GainEmber(burnAmount, Owner)` | 攻撃前に保持した火傷と同量の火種を獲得 |
| `DynamicVars.Damage.UpgradeValueBy(3m)` | 強化時のダメージ6→9 |

**参照元**：`TobiumesSlash.cs`（単体攻撃＋火種獲得）、`BlackCoffin.cs`（`GetPower<T>()` と `Amount`）、`EmberCmd.cs`（火種獲得・飛梅生成）。攻撃で対象が倒れても攻撃前の火傷数を火種獲得に使う。ソース実装済み、ビルド・ゲーム内動作未確認。


## 飛梅の火花（`TobiumesSpark.cs`）

| メソッド・変数 | 用途 |
|---|---|
| `new IntVar("Burn", 2)` | 敵全体へ付与する火傷数を表示・処理で共有 |
| `TargetType.AllEnemies` | カード対象を敵全体に設定 |
| `CombatState.GetOpponentsOf(Owner.Creature)` | 敵全体を取得 |
| `Where(enemy => enemy.IsAlive).ToList()` | 生存している敵のみを固定リストにする |
| `PowerCmd.Apply<BurnPower>(...)` | 各敵に火傷2（強化後3）を付与 |
| `DynamicVars["Burn"].UpgradeValueBy(1m)` | 強化後の火傷2→3 |

**参照元**：`TwinLotusBlueFirefall.cs`（敵全体への火傷付与と列挙）、`FlameBind.cs`（火傷変数と強化処理）。既存の`BurnPower`のターン開始ダメージ・ターン終了時減少は変更しない。ビルド・ゲーム内動作未確認。


## 炎の爆発（`FlameExplosion.cs`）

| メソッド・変数 | 用途 |
|---|---|
| `DamageVar(6m, ValueProp.Move)` | 通常攻撃のダメージ6を表示・処理で共有 |
| `IntVar("Triggers", 1)` | 火傷ダメージ即時発動回数。強化後2回 |
| `DamageCmd.Attack(...).FromCard(...).Targeting(...).Execute(...)` | 敵1体への通常攻撃 |
| `GetPower<BurnPower>()`・`burn.Amount` | 各回の現在の火傷スタックを参照 |
| `CreatureCmd.Damage(..., ValueProp.Unblockable \| ValueProp.Unpowered, null, null)` | `BurnPower`のターン開始時と同じ防御不能・非攻撃ダメージを即時発動 |
| `PowerCmd.Decrement(burn)` | 生存中の対象に即時発動1回につき火傷を1減らす |
| `DynamicVars["Triggers"].UpgradeValueBy(1m)` | 強化時の発動回数1→2 |

**参照元**：`BurnPower.cs`（火傷ダメージ・スタック減少）、`TobiumesFlameDance.cs`（攻撃・火傷参照）、`BlackCoffin.cs`（`GetPower<T>()`）。攻撃後に火傷ダメージを処理し、対象死亡または火傷0で停止。ビルド・ゲーム内動作未確認。


## 炎の吸収（`FlameAbsorption.cs`）

| メソッド・変数 | 用途 |
|---|---|
| `cardPlay.Target.GetPower<BurnPower>()`・`burn.Amount` | 対象の火傷数を取得 |
| `consumedBurn / 3` | 火傷3につき1HP、端数切り捨て |
| `PowerCmd.Remove(burn)` | 対象の火傷を全スタック消費 |
| `CreatureCmd.Heal(Owner.Creature, healAmount)` | 火傷消費量に応じてHP回復。**既存MOD内に使用例が見つからず、シグネチャ・ビルド未検証** |
| `EnergyCost.UpgradeBy(-1)` | 強化で2EN→1EN |

**参照元**：`AcceleratedCombustion.cs`（火傷参照）、`HinamoriModCard.cs`・`DebugResourceHelper.cs`（`PowerCmd.Remove`）、`AcceleratedCombustion.cs`（コスト強化）。`CreatureCmd.Heal`の実際のAPI適合はゲーム側アセンブリで要確認。ビルド・ゲーム内動作未確認。
