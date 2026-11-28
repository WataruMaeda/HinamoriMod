# RelicModel

完全修飾名 `MegaCrit.Sts2.Core.Models.RelicModel`

直接の基底型 `MegaCrit.Sts2.Core.Models.AbstractModel`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
protected LocString SelectionScreenPrompt { get; }
public abstract RelicRarity Rarity { get; }
public Player Owner { get; }
public void Flash();
public void Flash(IEnumerable<Creature> targets);
public virtual Task AfterObtained();
public virtual Task AfterRemoved();
```

## 役割・引数・戻り値とタイミング

- **Owner / Rarity / SelectionScreenPrompt** は所有者、レリックのレアリティ、選択画面プロンプト。Owner は所持状態で参照する。
- **Flash** はレリックの効果発動を表示する。引数ありのオーバーロードは対象を指定する。
- **AfterObtained / AfterRemoved** は取得・除去後のフック。戦闘中のフックは親クラス AbstractModel から継承する。
- マルチプレイでは Owner や Owner.Creature とフック引数を比較して、効果の対象を限定する。

## MOD 向け使用例

```csharp
public override Task AfterEnergyReset(Player player) =>
    player == Owner ? PlayerCmd.GainEnergy(1m, player) : Task.CompletedTask;
```

処理順と条件は [TurnLifecycle](../Mechanics/TurnLifecycle.md) を参照。
