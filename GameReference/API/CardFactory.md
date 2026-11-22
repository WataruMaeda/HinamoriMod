# CardFactory

完全修飾名 `MegaCrit.Sts2.Core.Factories.CardFactory`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public static IEnumerable<CardModel> GetDistinctForCombat(Player player, IEnumerable<CardModel> cards, int count, Rng rng);
public static IEnumerable<CardModel> GetForCombat(Player player, IEnumerable<CardModel> cards, int count, Rng rng);
public static IEnumerable<CardModel> FilterForCombat(IEnumerable<CardModel> cards);
public static IEnumerable<CardModel> GetDefaultTransformationOptions(CardModel original, bool isInCombat);
```

## 役割・引数・戻り値とタイミング

- **GetDistinctForCombat** は入力候補をラン・戦闘生成の条件で絞り、指定 RNG で重複を避けて戦闘カードを生成する。候補不足なら指定数を下回る。列挙結果は一度 ToList などで実体化して再利用する。
- **GetForCombat** は指定回数の抽選で同種カードが重複し得る。
- **FilterForCombat** は生成可否、レアリティ、重複に基づいて候補を絞る。
- **GetDefaultTransformationOptions** は元カードと isInCombat を基に変化先候補を返す。戦闘生成にはランの CombatCardGeneration RNG を渡す例が本体 Discovery にある。

## MOD 向け使用例

```csharp
var pool = player.Character.CardPool.GetUnlockedCards(
    player.UnlockState, player.RunState.CardMultiplayerConstraint);
var options = CardFactory.GetDistinctForCombat(
    player, pool, 3, player.RunState.Rng.CombatCardGeneration).ToList();
```

処理順と条件は [CardGeneration](../Mechanics/CardGeneration.md) を参照。
