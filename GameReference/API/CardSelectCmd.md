# CardSelectCmd

完全修飾名 `MegaCrit.Sts2.Core.Commands.CardSelectCmd`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public static Task<CardModel> FromChooseACardScreen(PlayerChoiceContext context, IReadOnlyList<CardModel> cards, Player player, bool canSkip = false);
public static Task<IEnumerable<CardModel>> FromSimpleGrid(PlayerChoiceContext context, IReadOnlyList<CardModel> cardsIn, Player player, CardSelectorPrefs prefs);
public static Task<IEnumerable<CardModel>> FromCombatPile(PlayerChoiceContext context, CardPile pile, Player player, CardSelectorPrefs prefs);
public static Task<IEnumerable<CardModel>> FromCombatPile(PlayerChoiceContext context, CardPile pile, Player player, CardSelectorPrefs prefs, Func<CardModel, bool> filter);
public static Task<IEnumerable<CardModel>> FromHand(PlayerChoiceContext context, Player player, CardSelectorPrefs prefs, Func<CardModel, bool> filter, AbstractModel source);
public static Task<IEnumerable<CardModel>> FromHandForDiscard(PlayerChoiceContext context, Player player, CardSelectorPrefs prefs, Func<CardModel, bool> filter, AbstractModel source);
```

## 役割・引数・戻り値とタイミング

- **FromChooseACardScreen** は候補リストから1枚選ぶ。canSkip はスキップ許可。結果は選択カードまたは null。
- **FromSimpleGrid / FromCombatPile** は候補リストまたは戦闘パイルから選んだカードを返す。prefs は選択数や表示、filter は対象の絞り込み。
- **FromHand / FromHandForDiscard** は指定プレイヤーの手札から選ぶ。source は効果の発生元。FromHandForDiscard は捨て札向け表示を設定して FromHand に委譲する。捨てる操作は CardCmd.Discard で行う。
- ローカル選択とリモート選択は同期される。全参加者で同じ候補順を作り、フックやカード効果の choiceContext を渡す。候補数が最小選択数以下の場合は設定により自動確定する。

## MOD 向け使用例

```csharp
var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1);
var cards = await CardSelectCmd.FromHandForDiscard(
    choiceContext, player, prefs, filter: null, source: this);
await CardCmd.Discard(choiceContext, cards);
```

処理順と条件は [CardGeneration](../Mechanics/CardGeneration.md) を参照。
