# PlayerCmd

完全修飾名 `MegaCrit.Sts2.Core.Commands.PlayerCmd`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public static Task GainEnergy(decimal amount, Player player);
public static Task LoseEnergy(decimal amount, Player player);
public static Task SetEnergy(decimal amount, Player player);
public static Task GainStars(decimal amount, Player player);
public static void EndTurn(Player player, bool canBackOut, Func<Task> actionDuringEnemyTurn = null);
```

## 役割・引数・戻り値とタイミング

- **GainEnergy / LoseEnergy / SetEnergy** は指定プレイヤーの現在 EN を増加・減少・設定する Task API。amount は量。**GainStars** はスターを獲得する。
- **EndTurn** はターン終了を要求する。canBackOut は終了準備を取り消せるか、actionDuringEnemyTurn は敵ターン側で実行する任意処理。これは void API。
- ターンごとの EN 追加は AfterEnergyReset で所有者を絞る。

## MOD 向け使用例

```csharp
public override Task AfterEnergyReset(Player player) =>
    player == Owner ? PlayerCmd.GainEnergy(1m, player) : Task.CompletedTask;
```

処理順と条件は [TurnLifecycle](../Mechanics/TurnLifecycle.md) を参照。
