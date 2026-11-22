# AbstractModel

完全修飾名 `MegaCrit.Sts2.Core.Models.AbstractModel`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public virtual Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card);
public virtual Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw);
public virtual Task AfterCardGeneratedForCombat(CardModel card, Player creator);
public virtual Task BeforeCardAutoPlayed(CardModel card, Creature target, AutoPlayType type);
public virtual Task BeforeCardPlayed(CardPlay cardPlay);
public virtual Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay);
public virtual Task BeforeCombatStart();
public virtual Task AfterEnergyReset(Player player);
public virtual Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState);
public virtual Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState);
public virtual Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState);
public virtual Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player);
public virtual Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants);
public virtual decimal ModifyDamageAdditive(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource, CardPlay cardPlay);
public virtual decimal ModifyDamageCap(Creature target, ValueProp props, Creature dealer, CardModel cardSource, CardPlay cardPlay);
public virtual decimal ModifyDamageMultiplicative(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource, CardPlay cardPlay);
public virtual decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature target, CardModel cardSource);
public virtual decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature target, CardModel cardSource);
public virtual bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost);
public virtual bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost);
public virtual bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost);
public virtual bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature applier, out decimal modifiedAmount);
```

## 役割・引数・戻り値とタイミング

- **BeforeCombatStart / BeforeSideTurnStart / AfterSideTurnStart / AfterSideTurnEnd** は戦闘とサイドターンのフック。participants はそのサイドターンに参加するクリーチャー。
- **AfterEnergyReset / BeforeHandDraw / AfterPlayerTurnStart** はプレイヤーごとの準備フック。選択を伴う初手効果は AfterPlayerTurnStart のコンテキストを使う。
- **BeforeCardAutoPlayed / BeforeCardPlayed / AfterCardPlayed** はカード実行前後。CardPlay を受け取るフックではシリーズ内の実行回を判定できる。**AfterCardDrawn / AfterCardGeneratedForCombat / AfterCardDiscarded** はドロー・生成・捨て札の通知。
- **TryModifyEnergyCostInCombat / TryModifyEnergyCostInCombatLate / TryModifyStarCost** の out は補正後コスト、bool は補正を適用するかどうか。
- **ModifyDamageAdditive / Multiplicative / Cap** はそれぞれ加算分・倍率・上限を返す。target、dealer、cardSource、cardPlay は文脈により null になり得る。
- **ModifyPowerAmountGivenAdditive / Multiplicative** は付与側の加算分・倍率。**TryModifyPowerAmountReceived** は受け手側で補正後の値を out に返す。

## MOD 向け使用例

```csharp
public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
{
    if (player == Owner && player.PlayerCombatState?.TurnNumber == 1)
        await CardPileCmd.Draw(context, player);
}
```

処理順と条件は [TurnLifecycle](../Mechanics/TurnLifecycle.md) を参照。
