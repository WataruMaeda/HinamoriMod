# MayhemPower（騒乱）

## DLL で確認した API と処理順

完全修飾名は `MegaCrit.Sts2.Core.Models.Powers.MayhemPower`。基底型は `PowerModel`。

```csharp
public override Task AfterAutoPrePlayPhaseEntered(
    PlayerChoiceContext choiceContext, Player player);
```

1. 自動プレイ前フェーズで、フックの player がパワー所有者の場合に発動する。
2. `MayhemPower.<AfterAutoPrePlayPhaseEntered>d__4.MoveNext` の IL_0042 で `CardPileCmd.AutoPlayFromDrawPile(choiceContext, Owner.Player, Amount, CardPilePosition.Top, false)` を呼ぶ。
3. 山札から先頭カードを選び、必要に応じたシャッフルやパイル移動を経て `CardCmd.AutoPlay` に `AutoPlayType.Default` を渡す。
4. 実行される経路では `CardCmd.<AutoPlay>d__0.MoveNext` の IL_0616 が true を積み、`OnPlayWrapper` に渡す。ラッパーは IL_0590 で `CardPlay.IsAutoPlay` に設定する。カードの効果が実行される場合はこの `CardPlay` を受け取る。

`CardPlay` は騒乱固有の発生元を保持しない。自動プレイ全般を対象にするなら `IsAutoPlay` を使う。

## MOD 向け使用例

詠唱を追加で支払う処理の先頭で使う例。雛森 MOD の `SpendChant` にある条件。

```csharp
if (cardPlay.IsAutoPlay)
    return true;
```

詳細は [CardPlayFlow](../Mechanics/CardPlayFlow.md) と [CardPlay](../API/CardPlay.md)。
