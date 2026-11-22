# CardCmd

完全修飾名 `MegaCrit.Sts2.Core.Commands.CardCmd`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public static Task AutoPlay(PlayerChoiceContext choiceContext, CardModel card, Creature target, AutoPlayType type = AutoPlayType.Default, bool skipXCapture = false, bool skipCardPileVisuals = false);
public static Task Discard(PlayerChoiceContext choiceContext, CardModel card);
public static Task Discard(PlayerChoiceContext choiceContext, IEnumerable<CardModel> cards);
public static Task DiscardAndDraw(PlayerChoiceContext choiceContext, IEnumerable<CardModel> cardsToDiscard, int cardsToDraw);
public static Task<CardPileAddResult?> Exhaust(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal = false, bool skipVisuals = false);
public static void Upgrade(CardModel card, CardPreviewStyle style = CardPreviewStyle.HorizontalLayout);
public static void Upgrade(IEnumerable<CardModel> cards, CardPreviewStyle style);
public static TaskCompletionSource Preview(CardModel card, float time = 1.2f, CardPreviewStyle style = CardPreviewStyle.HorizontalLayout);
public static void Preview(IReadOnlyList<CardModel> cards, float time = 1.2f, CardPreviewStyle style = CardPreviewStyle.HorizontalLayout);
```

## 役割・引数・戻り値とタイミング

- **AutoPlay** はカード、対象、実行種別を受け取り、自動プレイの経路を走らせる。対象未指定時の処理はカードの対象タイプに依存する。skipXCapture は X の捕捉、skipCardPileVisuals はパイル移動の表示を制御する。実行前の禁止条件により効果が発動しない場合がある。
- **Discard / DiscardAndDraw** はカードを捨てる。後者は捨てた後に指定枚数を引く。選択処理は CardSelectCmd。
- **Exhaust** はカードを廃棄し、パイル追加結果を返す。causedByEthereal は Ethereal による廃棄を示す。
- **Upgrade / Preview** はカード強化とプレビュー。style は表示形式、time はプレビュー表示時間。

## MOD 向け使用例

```csharp
await CardCmd.Discard(choiceContext, selectedCards);
```

処理順と条件は [CardPlayFlow](../Mechanics/CardPlayFlow.md) を参照。
