# 戦闘中のカード生成

## DLL で確認した流れ

1. 本体 `Discovery.OnPlay` は解禁済みプールを取得し、ランの `CombatCardGeneration` RNG を `CardFactory.GetDistinctForCombat` に渡す。Factory はランと戦闘の条件で絞り込み、抽選後に `ICombatState.CreateCard` で可変の戦闘カードを作る。
2. `CardSelectCmd.FromChooseACardScreen` は候補インデックスを同期する。ローカル側は選択画面を表示し、リモート側は同期された選択を待つ。MOD では候補を一度実体化し、同じ候補順と実行中の `PlayerChoiceContext` を使う。
3. `CardPileCmd.AddGeneratedCardToCombat` は複数枚版へ進み、戦闘パイルを検査し、生成履歴を記録して `Add` を呼ぶ。その後 `AfterCardGeneratedForCombat` を通知する。手札が上限なら `Add` の経路で捨て札に回る。
4. `FromHandForDiscard` → `CardCmd.Discard` でその時点の手札から選んで捨てられる。

## MOD 向け使用例

```csharp
var pool = player.Character.CardPool.GetUnlockedCards(
    player.UnlockState, player.RunState.CardMultiplayerConstraint);
var candidates = CardFactory.GetDistinctForCombat(
    player, pool, 3, player.RunState.Rng.CombatCardGeneration).ToList();
if (candidates.Count == 0)
    return;

var card = await CardSelectCmd.FromChooseACardScreen(
    choiceContext, candidates, player);
if (card != null)
    await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
```

本体の状態異常カードが必要なら `ModelDb.CardPool<StatusCardPool>().AllCards` から、本体 DLL に定義された型を選び、`Rng.NextItem` → `ICombatState.CreateCard(CardModel, Player)` → `AddGeneratedCardToCombat` と進める。 `AllCards` は MOD から追加されたモデルを含み得る。

詳細は [CardFactory](../API/CardFactory.md)、[CardSelectCmd](../API/CardSelectCmd.md)、[CardPileCmd](../API/CardPileCmd.md)。
