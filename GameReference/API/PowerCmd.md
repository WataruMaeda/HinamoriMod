# PowerCmd

完全修飾名 `MegaCrit.Sts2.Core.Commands.PowerCmd`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public static Task<IReadOnlyList<T>> Apply<T>(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, decimal amount, Creature applier, CardModel cardSource, bool silent = false) where T : PowerModel;
public static Task<T> Apply<T>(PlayerChoiceContext choiceContext, Creature target, decimal amount, Creature applier, CardModel cardSource, bool silent = false) where T : PowerModel;
public static Task Apply(PlayerChoiceContext choiceContext, PowerModel power, Creature target, decimal amount, Creature applier, CardModel cardSource, bool silent = false);
public static PowerModel FindExistingInstanceForStacking(PowerModel basePower, Creature target, Creature applier);
public static Task Decrement(PowerModel power);
public static Task TickDownDuration(PowerModel power);
public static Task<int> ModifyAmount(PlayerChoiceContext choiceContext, PowerModel power, decimal offset, Creature applier, CardModel cardSource, bool silent = false);
public static Task Remove<T>(Creature creature) where T : PowerModel;
public static Task Remove(PowerModel power);
```

## 役割・引数・戻り値とタイミング

- **Apply** は対象、付与量、付与者、発生元カードを取り、パワーを付与する。複数対象版は結果一覧を返す。単体 generic 版の結果は受け取れない場合などに null になり得る。silent は付与時の表示用。
- **FindExistingInstanceForStacking** は既存のスタック先を調べる。付与者も判定に関わる。
- **ModifyAmount** は増減分を受け取り、新しい量を返す。**Decrement** は −1 を渡す。**TickDownDuration** は SkipNextDurationTick を確認してから減少させる。
- **Remove** は対象のパワーを外す。選択を伴いうる経路では受け取った choiceContext を引き渡し Task を待つ。

## MOD 向け使用例

```csharp
await PowerCmd.Apply<WeakPower>(choiceContext, target, 2m, Owner.Creature, this);
```

処理順と条件は [PowerApplication](../Mechanics/PowerApplication.md) を参照。
