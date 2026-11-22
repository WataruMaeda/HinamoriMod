# CardPileCmd

完全修飾名 `MegaCrit.Sts2.Core.Commands.CardPileCmd`

直接の基底型 `System.Object`

## DLL で確認したメンバー

MOD で使う主な public / protected メンバーを抜粋。CLR シグネチャを C# 風に整形し、参照型の null 許容注釈は省略しています。

```csharp
public static Task<CardPileAddResult> AddGeneratedCardToCombat(CardModel card, PileType newPileType, Player creator, CardPilePosition position = CardPilePosition.Bottom);
public static Task<IReadOnlyList<CardPileAddResult>> AddGeneratedCardsToCombat(IEnumerable<CardModel> cards, PileType newPileType, Player creator, CardPilePosition position = CardPilePosition.Bottom);
public static Task<CardPileAddResult> Add(CardModel card, PileType newPileType, CardPilePosition position = CardPilePosition.Bottom, AbstractModel clonedBy = null, bool skipVisuals = false);
public static Task<CardPileAddResult> Add(CardModel card, CardPile newPile, CardPilePosition position = CardPilePosition.Bottom, AbstractModel clonedBy = null, bool skipVisuals = false);
public static Task<IReadOnlyList<CardPileAddResult>> Add(IEnumerable<CardModel> cards, PileType newPileType, CardPilePosition position = CardPilePosition.Bottom, AbstractModel clonedBy = null, bool skipVisuals = false);
public static Task<IReadOnlyList<CardPileAddResult>> Add(IEnumerable<CardModel> cards, CardPile newPile, CardPilePosition position = CardPilePosition.Bottom, AbstractModel clonedBy = null, bool skipVisuals = false, bool isChangingOwners = false);
public static Task<CardModel> Draw(PlayerChoiceContext choiceContext, Player player);
public static Task<IEnumerable<CardModel>> Draw(PlayerChoiceContext choiceContext, decimal count, Player player, bool fromHandDraw = false);
public static Task AutoPlayFromDrawPile(PlayerChoiceContext choiceContext, Player player, int count, CardPilePosition position, bool forceExhaust);
public static Task AutoPlayFromDrawPile(PlayerChoiceContext choiceContext, Player player, int count, CardPilePosition position, bool forceExhaust, Action<CardModel> cardModification);
public static Task ShuffleIfNecessary(PlayerChoiceContext choiceContext, Player player);
public static Task AddToCombatAndPreview<T>(IEnumerable<Creature> targets, PileType pileType, int count, Player creator, CardPilePosition position = CardPilePosition.Bottom) where T : CardModel;
public static Task AddToCombatAndPreview<T>(Creature target, PileType pileType, int count, Player creator, CardPilePosition position = CardPilePosition.Bottom) where T : CardModel;
```

## 役割・引数・戻り値とタイミング

- **AddGeneratedCardToCombat / AddGeneratedCardsToCombat** は戦闘用カードを指定パイルへ移し、生成履歴と生成フックを扱う。creator は生成者、カードの所有者はカード側に保持される。結果は CardPileAddResult。
- **Add** はカードを指定パイルへ移動する。位置、クローン元、演出の省略を指定できる。手札が満杯の場合は捨て札に送られる。
- **Draw** は指定プレイヤーのドローを行い、引いたカードを返す。fromHandDraw はターンの手札ドロー経由であることを示す。**ShuffleIfNecessary** は必要に応じて山札を補充する。
- **AutoPlayFromDrawPile** は位置と枚数を指定して山札から自動プレイする。forceExhaust は廃棄を強制する指定。追加オーバーロードで事前のカード変更を渡せる。
- **AddToCombatAndPreview** は型で指定したカードを戦闘中に生成して表示する。

## MOD 向け使用例

```csharp
await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
```

処理順と条件は [CardGeneration](../Mechanics/CardGeneration.md) を参照。
