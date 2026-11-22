# CardModel

完全修飾名 `MegaCrit.Sts2.Core.Models.CardModel`

直接の基底型 `MegaCrit.Sts2.Core.Models.AbstractModel`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public Player Owner { get; }
public CardPile Pile { get; }
protected virtual bool HasEnergyCostX { get; }
public CardEnergyCost EnergyCost { get; }
protected virtual bool IsPlayable { get; }
public ICombatState CombatState { get; }
public void SetToFreeThisTurn();
public int GetStarCostWithModifiers();
protected virtual Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay);
protected virtual void OnUpgrade();
public bool CanPlayTargeting(Creature target);
public bool CanPlay();
public bool CanPlay(out UnplayableReason reason, out AbstractModel preventer);
public Task<ValueTuple<int, int>> SpendResources();
public Task OnPlayWrapper(PlayerChoiceContext choiceContext, Creature target, bool isAutoPlay, ResourceInfo resources, bool skipCardPileVisuals = false);
```

## 役割・引数・戻り値とタイミング

- **Owner / Pile / CombatState** は所有者・現在のパイル・戦闘状態。戦闘外や未配置時は Pile / CombatState の有無を確認する。**EnergyCost / HasEnergyCostX / GetStarCostWithModifiers** は EN コストオブジェクト、X 判定、補正後スターコストを返す。
- **CanPlay / CanPlayTargeting / IsPlayable** は使用可否の判定。理由と阻害したモデルを返すオーバーロードもある。MOD の追加コスト要件は IsPlayable で検査できる。
- **OnPlay** は効果を実行する protected フックで、選択コンテキストとその回の CardPlay を受け取る。**OnUpgrade** は強化実装用。
- **SpendResources** は EN・スターを支払い、支払った値の組を返す。**OnPlayWrapper** はプレイ全体を進める実行入口。通常のカード実装では OnPlay をオーバーライドする。
- **SetToFreeThisTurn** はそのターンの一時コストを設定する。ゲーム外の MOD 独自コストに適用する場合は別途判定する。

## MOD 向け使用例

```csharp
protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
{
    await PlayerCmd.GainEnergy(1m, Owner);
}
```

処理順と条件は [CardPlayFlow](../Mechanics/CardPlayFlow.md) を参照。
