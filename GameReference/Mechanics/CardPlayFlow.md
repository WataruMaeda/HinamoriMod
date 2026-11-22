# カード使用の流れ

## DLL で確認した手動プレイ

1. `PlayCardAction.ExecuteAction` がネットワーク上のカード参照をカード実体に解決し、パイル、`CardModel.CanPlay`、対象の有効性を確認する。失敗時はアクションを取り消す経路がある。
2. `CardModel.SpendResources` を待機し、支払い情報から `ResourceInfo` を組み立てる。
3. `CardModel.OnPlayWrapper` に `isAutoPlay: false` を渡す。ラッパーが `CardPileCmd.AddDuringManualCardPlay` を呼び、結果パイルと実行回数を計算する。
4. 実行回ごとに `CardPlay` を生成し、`Hook.BeforeCardPlayed` → 戦闘履歴の開始記録 → `CardModel.OnPlay` → エンチャント・アフリクションの効果 → 戦闘履歴の終了記録 → `Hook.AfterCardPlayed` の順に進む。各段階には戦闘が継続しているかなどの条件がある。
5. 変化、所有者移動、戦闘からの除去、廃棄、結果パイルへの移動などを分岐処理し、空の手札と一時コストを後処理する。

`PlayCardAction.<ExecuteAction>.MoveNext` の主要呼び出しは IL_0181、IL_02c1、IL_0388。 `CardModel.<OnPlayWrapper>.MoveNext` の前フック、効果、後フックは IL_05be、IL_0675、IL_08a3。

## DLL で確認した自動プレイ

`CardCmd.AutoPlay` は使用禁止や対象不足で、カードの効果を発動せず結果パイルへ送る場合がある。実行する経路では `Hook.BeforeCardAutoPlayed` を経由し、`OnPlayWrapper` に `isAutoPlay: true` を渡す。騒乱の開始位置は [MayhemPower](../Powers/MayhemPower.md) を参照。

MOD では効果本体を `OnPlay` に置き、シリーズ単位の処理には `CardPlay.IsFirstInSeries` または `IsLastInSeries` を使う。選択を行うときは渡された `PlayerChoiceContext` を使う。関連 API は [CardModel](../API/CardModel.md)、[CardPlay](../API/CardPlay.md)、[CardCmd](../API/CardCmd.md)。
