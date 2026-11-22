# VoidFormPower（虚無化）

## DLL で確認した API

完全修飾名は `MegaCrit.Sts2.Core.Models.Powers.VoidFormPower`。基底型は `PowerModel`。本体 `Models.Cards.VoidForm.OnPlay` は基本値 2 のパワーを付与し、ターン終了を要求する。

```csharp
public override bool TryModifyEnergyCostInCombatLate(
    CardModel card, decimal originalCost, out decimal modifiedCost);

public override bool TryModifyStarCost(
    CardModel card, decimal originalCost, out decimal modifiedCost);

public override Task BeforeSideTurnStart(
    PlayerChoiceContext choiceContext, CombatSide side,
    IReadOnlyList<Creature> participants, ICombatState combatState);

public override Task AfterCardPlayed(
    PlayerChoiceContext choiceContext, CardPlay cardPlay);
```

- 両コストフックは private の `ShouldSkip(CardModel)` を使う。所有者のカードが Hand または Play にあり、内部カウンターが Amount 未満なら true と補正後コスト 0 を返す。この呼び出し自体はカウンターを消費しない。
- 所有者がターン参加者にいると `BeforeSideTurnStart` でカウンターが 0 になる。 `AfterCardPlayed` は所有者の手動プレイのシリーズ末尾で 1 加算する。付与・スタック変更時には `HideTemporaryZeroCostVisual` 経由でカウンターが一時的に大きな値となる。
- 無料の手動プレイは `PlayCardAction` を経由し、`IsAutoPlay` は false。 `CardModel` と `CardPlay` には虚無化による無料化を記録する専用メンバーがない。 `EnergySpent == 0` は元から 0 EN のカードにも成立する。
- 固定 EN の支払いは late 補正を経由する。X コストの支払いは `CardEnergyCost.GetAmountToSpend` の別分岐で現在 EN を読む。

## MOD 向け使用例

次の判定は `HinamoriModCard` のインスタンス内に置く。虚無化フックの bool を見て、この効果の適用可否を判定する。

```csharp
private bool IsVoidFormFreeNow =>
    ChantCost >= 0 && IsMutable && CombatState != null && !HasEnergyCostX &&
    Owner.Creature.GetPower<VoidFormPower>()
        ?.TryModifyEnergyCostInCombatLate(this, 0m, out _) == true;
```

1. `EffectiveChantCost` と使用可否では、この判定が true の場合に詠唱コストを 0 として表示・評価する。
2. `CardModel.SpendResources()` の Harmony Prefix で、対象の雛森カードに判定結果を記録する。毎回代入して前回の判定を上書きする。
3. `SpendChant` は既存のリプレイと自動プレイのガードの後でその記録を使い、無料なら true を返し消費を省略する。通常ルートの消費額には、虚無化のリアルタイム判定を追加する前の詠唱計算を使う。
4. 黒棺は固定 `ChantCost` が負で、`OnPlay` に独自の全詠唱消費を持つ。

詳細は [CostCalculation](../Mechanics/CostCalculation.md)。上記は MOD 実装の提案で、適用や実行の結果は未確認。
