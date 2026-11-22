# CardPlay

完全修飾名 `MegaCrit.Sts2.Core.Entities.Cards.CardPlay`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public CardModel Card { get; init; }
public Player Player { get; init; }
public Creature Target { get; init; }
public PileType ResultPile { get; init; }
public ResourceInfo Resources { get; init; }
public bool IsAutoPlay { get; init; }
public int PlayIndex { get; init; }
public int PlayCount { get; init; }
public bool IsFirstInSeries { get; }
public bool IsLastInSeries { get; }
```

## 役割・引数・戻り値とタイミング

- **Card / Player / Target** はプレイされたカード、実行プレイヤー、対象。対象不要の効果などでは Target が null になり得る。
- **Resources** は EnergySpent、EnergyValue、StarsSpent、StarValue を持つ。支払いゼロの理由は保持しない。
- **ResultPile** は実行時に計算された行き先。**IsAutoPlay** は OnPlayWrapper に渡された自動プレイフラグ。
- **PlayIndex / PlayCount / IsFirstInSeries / IsLastInSeries** は同じプレイ内での実行回を表す。追加コストの支払いを初回に限定できる。

## MOD 向け使用例

```csharp
if (play.IsFirstInSeries && !play.IsAutoPlay)
{
    // MOD 独自の追加コストをここで処理する。
}
```

処理順と条件は [CardPlayFlow](../Mechanics/CardPlayFlow.md) を参照。
