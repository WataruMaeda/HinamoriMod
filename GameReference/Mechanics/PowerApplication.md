# パワー付与とスタック

## DLL で確認した流れ

1. `PowerCmd.Apply<T>` は戦闘終了や対象の受け取り可否を確認し、`ModelDb.Power<T>` と `FindExistingInstanceForStacking` を使う。初回は可変インスタンスを作り `Apply`、既存なら `ModifyAmount` へ進む。複数対象版は対象ごとに単体版を使う。
2. 新規 `Apply` は `BeforePowerAmountChanged` → 付与側の加算・乗算 → 受け手側の補正 → 条件つきのマルチプレイ補正 → `PowerModel.BeforeApplied` → `ApplyInternal` の順。付与側補正は applier が戦闘内にいる場合に呼ばれる。
3. 新規付与後は条件に応じて履歴、`AfterModifyingPowerAmountGiven`、`AfterModifyingPowerAmountReceived`、`PowerModel.AfterApplied`、`AfterPowerAmountChanged` を処理する。
4. `ModifyAmount` にも付与側と受け手側の補正が入る。補正された差分を整数に変換して現在値に足し、`SetAmount` を呼ぶ。除去条件は `ShouldRemoveDueToAmount` で検査する。

初回経路は `PowerCmd.<Apply>d__2.MoveNext`、変更経路は `PowerCmd.<ModifyAmount>d__6.MoveNext`。

## MOD 向け使用例

レリック所有者が与える雛森 MOD の正の火傷を +1 する例。 `Decrement` 自体も `ModifyAmount` へ進むため、正の付与に限定する。付与者が null の経路まで対象にする場合は受け手側の `TryModifyPowerAmountReceived` を検討する。

```csharp
public override decimal ModifyPowerAmountGivenAdditive(
    PowerModel power, Creature giver, decimal amount,
    Creature? target, CardModel? cardSource)
{
    return power is BurnPower && giver == Owner.Creature && amount > 0m ? 1m : 0m;
}
```

引数の詳細は [PowerCmd](../API/PowerCmd.md)、フックは [AbstractModel](../API/AbstractModel.md)。
