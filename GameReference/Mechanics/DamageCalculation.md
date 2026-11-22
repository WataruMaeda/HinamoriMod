# ダメージ計算

## DLL で確認した流れ

1. `CreatureCmd.Damage` の共通経路から `Hook.ModifyDamage` を呼ぶ。エンチャント由来の補正経路を持つ。
2. `Hook.ModifyDamageInternal` は有効なフック種別に応じて、`ModifyDamageAdditive` → `ModifyDamageMultiplicative` → `ModifyDamageMultiplicativeLate` → `ModifyDamageCap` の順で適用する。加算フックは差分、乗算フックは係数を返す。
3. `AfterModifyingDamageAmount`、`BeforeDamageReceived` を経由し、`ModifyPreBlockDamageTarget` で対象が変わる経路がある。通常経路は `Creature.DamageBlockInternal` でブロックを処理する。
4. 残りのダメージは HP 損失の補正と振り分けを経て `Creature.LoseHpInternal` へ進む。`ModifyHpLost` と `ModifyUnblockedDamageTarget` はこの段階に関わる。履歴を記録し、条件に応じて `AfterBlockBroken`、`AfterCurrentHpChanged`、`AfterDamageGiven`、`AfterDamageReceived` を呼ぶ。

`CreatureCmd.<Damage>d__13.MoveNext` では倍率を含む `Hook.ModifyDamage` が IL_01ce、ブロック処理が IL_03f0。分岐によっては別の HP 損失経路を通る。

## MOD 向け使用例

レリック所有者が受ける攻撃をブロック計算の前に 20% 軽減する例。 `IsPoweredAttack()` は `Move` があり `Unpowered` が無い属性を判定する。

```csharp
public override decimal ModifyDamageMultiplicative(
    Creature? target, decimal amount, ValueProp props,
    Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
{
    return target == Owner.Creature && props.IsPoweredAttack() ? 0.8m : 1m;
}
```

## 未確認

すべての対象振り分け、Osty 関連の例外経路、攻撃プレビューの全分岐。
