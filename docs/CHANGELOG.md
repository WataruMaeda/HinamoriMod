## 2026-11-20：鬼道の鍛錬を実装（動作未確認）

- **確定仕様**：アンコモン・2EN。すべての鬼道の詠唱コスト−1（最低0）。強化後は天賦。
- **追加**：`Cards/KidoDiscipline.cs`、`Powers/KidoDisciplinePower.cs`。
- **変更**：`Cards/HinamoriModCard.cs` の `EffectiveChantCost` に鬼道の鍛錬の軽減を追加。破道専用の軽減と加算し、最低0を維持。
- **ローカライズ**：日本語・英語の `cards.json` と `powers.json` に名称・効果を追加。
- **ドキュメント**：`docs/CardList.md` と `docs/ImplementedMethodReference.md` を更新。
- **理由**：既存の詠唱消費処理と矛盾せず、複数回使用した際にも軽減を累積させるため。
- **検証**：GitHubへの反映のみ確認。**ビルド・ゲーム内動作は未確認**。特に `AddKeyword(CardKeyword.Innate)` のAPI互換性とカード表示の更新を要確認。

## 2026-11-20：炎の吸収をレア・B案で確定し実装

- **確定仕様**：レアのスキル、通常2EN・強化後1EN。敵1体の火傷をすべて消費し、火傷3につきHPを1回復（端数切り捨て）。最大HPを超えて回復しない。廃棄なし。C案の火傷2につき1HP回復は不採用。
- **追加**：`HinamoriModCode/Cards/FlameAbsorption.cs`。`GetPower<BurnPower>()`で火傷数を取得し、`PowerCmd.Remove`で全スタック消費、消費数÷3を`CreatureCmd.Heal`へ渡す。`EnergyCost.UpgradeBy(-1)`で強化時にコストを下げる。日本語コメントを記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json`にカード名・説明文を追加。
- **更新**：`docs/CardList.md`にレアリティ・コスト・強化仕様・実装状況`実装済・未確認`を反映。
- **更新**：`docs/ImplementedMethodReference.md`にメソッドの用途と未検証の回復APIを記録。
- **理由**：火傷をすべて失う対価とHP回復のバランスを考慮し、ユーザー確定のB案とレアリティに合わせるため。
- **検証**：GitHubコミットと日本語・英語JSONの形式解析を確認。**ビルド・ゲーム内動作は未確認**。`CreatureCmd.Heal(Owner.Creature, healAmount)`はMOD既存ソースで呼び出し例を確認できておらず、シグネチャ確認・ビルドが必要。

## 2026-11-20：炎の爆発の仕様確定・実装

- **確定仕様**：アンコモンのアタック、1EN、敵1体に6ダメージを与え、対象の火傷ダメージを即座に1回発動し火傷を1減らす。強化後は即時発動2回、各回火傷を1減らす。
- **追加**：`HinamoriModCode/Cards/FlameExplosion.cs`。`DamageCmd.Attack` に続き、各回の `BurnPower.Amount` を参照して `CreatureCmd.Damage`（防御不能・非攻撃）で火傷ダメージを即時発動し、対象生存中なら `PowerCmd.Decrement` で火傷を1減らす。火傷0または対象死亡で残りの発動を停止。日本語コメントを記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json` に名称と即時発動回数が強化で変化する説明文を追加。
- **更新**：`docs/CardList.md` の実装状況を `実装済・未確認` に変更。
- **更新**：`docs/ImplementedMethodReference.md` に使用メソッドと参照元を追記。
- **理由**：既存の火傷のターン開始時ダメージと同じ計算方式で、カード使用時の即時発動を再現するため。
- **検証**：GitHubコミットと日本語・英語JSONの解析を確認。**ビルド・ゲーム内動作は未確認**。

## 2026-11-20：飛梅の火花の仕様変更・実装

- **確定仕様**：B案。コモンのスキル、1EN、敵全体に火傷2を付与。強化後は敵全体に火傷3を付与。旧案の単体火傷3（強化後5）は不採用。
- **追加**：`HinamoriModCode/Cards/TobiumesSpark.cs`。`TargetType.AllEnemies` と `CombatState.GetOpponentsOf` で生存中の敵全体を対象にし、`PowerCmd.Apply<BurnPower>` を敵ごとに呼んで火傷を付与。`IntVar("Burn", 2)` を強化時に+1。処理意図を日本語コメントで記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json` にカード名と敵全体への火傷付与説明を追加。
- **更新**：`docs/CardList.md` に確定仕様・レアリティ・実装状況 `実装済・未確認` を反映。
- **更新**：`docs/ImplementedMethodReference.md` に使用メソッドと既存実装の参照元を追記。
- **理由**：単体火傷3のみでは既存の「炎の拘束」等との差別化が弱いため、全体火傷付与へ変更する。
- **検証**：GitHubコミットと日本語・英語JSONの形式解析を確認。**ビルド・ゲーム内動作は未確認**。

## 2026-11-20：飛梅の炎舞の仕様確定・実装

- **確定仕様**：アンコモンのアタック、1EN、敵1体に6ダメージを与え、対象の火傷数と同量の火種を獲得。強化後は9ダメージ、火種獲得量は同じ。
- **追加**：`HinamoriModCode/Cards/TobiumesFlameDance.cs`。`GetPower<BurnPower>()?.Amount` で攻撃前の火傷スタック数を取得し、`DamageCmd.Attack` で攻撃後、`EmberCmd.GainEmber` で同量の火種を獲得。火傷がない敵は0扱い。攻撃で敵が倒れた場合も攻撃前の火傷数を使う。日本語コメントを記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json` と `HinamoriMod/localization/eng/cards.json` にカード名・説明文を追加。
- **更新**：`docs/CardList.md` にレアリティ・強化効果・実装状況 `実装済・未確認` を反映。
- **更新**：`docs/ImplementedMethodReference.md` に使用メソッドと既存ソースの参照元を追記。
- **理由**：火傷を蓄積した敵から火種へつなぐコンボを、既存の攻撃・火種管理処理で実装するため。
- **確認状況**：GitHubコミットおよび日本語・英語JSONの解析を確認。**ビルド・ゲーム内動作は未確認**。

## 2026-11-20：燃焼促進の仕様確定・実装

- **確定仕様**：アンコモンのスキル、2EN、敵1体の火傷スタックを2倍にして廃棄。強化後は1EN、火傷2倍・廃棄は維持。
- **追加**：`HinamoriModCode/Cards/AcceleratedCombustion.cs`。対象の既存 `BurnPower` を `GetPower<BurnPower>()` で取得し、現在のスタックと同じ量を `PowerCmd.Apply<BurnPower>` で追加して2倍にする。火傷がない場合は何もしない。`CanonicalKeywords` に廃棄を指定し、`EnergyCost.UpgradeBy(-1)` で強化コストを下げる。処理意図を日本語コメントで記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json` にカード名と説明文を追加。
- **更新**：`docs/CardList.md` のレアリティ・強化仕様・実装状況を `実装済・未確認` に更新。
- **更新**：`docs/ImplementedMethodReference.md` に既存メソッドの参照元と用途を追記。
- **理由**：合意した火傷倍化コンボカードを既存の火傷パワー処理と整合させて実装するため。
- **検証**：GitHubへの反映と日本語・英語JSONの解析を確認。**ビルド・ゲーム内動作は未確認**。

## 2026-11-20：炎の拘束の仕様確定・実装

- **確定仕様**：コモンのスキル、1EN、敵1体へ火傷2、自分に5ブロック。強化後は火傷3、5ブロック。
- **追加**：`HinamoriModCode/Cards/FlameBind.cs`。既存の `PowerCmd.Apply<BurnPower>` と `CreatureCmd.GainBlock` を組み合わせ、対象への火傷付与→自分のブロック獲得の順で処理。`GainsBlock` を有効化し、火傷数だけ強化。処理意図を日本語コメントで記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json` に名称と説明を追加。前回実装の「飛梅の炎撃」の説明文にあった二重エスケープ改行も両言語で修正。
- **更新**：`docs/CardList.md` のレアリティ・強化効果・実装状況を `実装済・未確認` に変更。
- **更新**：`docs/ImplementedMethodReference.md` に使用メソッドと既存実装の参照元を追記。
- **理由**：確定した火傷＋ブロックの複合スキルを既存のカード処理で再現するため。
- **検証**：JSONをプログラムで解析して形式を確認。GitHubコミット完了。**ビルド・ゲーム内動作は未確認**。

## 2026-11-20：飛梅の炎撃の仕様確定・実装

- **確定仕様**：コモン、2EN、敵1体に12ダメージ＋火傷3。強化後は12ダメージ＋火傷5。
- **追加**：`HinamoriModCode/Cards/TobiumesFlameStrike.cs`。既存の単体攻撃と `PowerCmd.Apply<BurnPower>` を利用し、攻撃後に対象が生存している場合のみ火傷を付与。強化では火傷数のみ増加。日本語コメントで処理意図を記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json` に名称・説明を追加。英語版の「炎の守り」に残っていた改行の二重エスケープも修正。
- **更新**：`docs/CardList.md` のレアリティ・強化効果を確定し、実装状況を `実装済・未確認` に変更。
- **更新**：`docs/ImplementedMethodReference.md` に既存の火傷付与メソッドの利用例を追記。
- **理由**：合意したカード性能を既存の火傷システムに合わせて実装するため。
- **確認状況**：GitHubへの反映とJSON形式の生成を確認。**ビルド・ゲーム内動作未確認**。

## 2026-11-20：日本語カード説明文を本体表記に統一

- **更新**：`HinamoriMod/localization/jpn/cards.json` の通常説明文・強化後説明文を見直し。単体攻撃の冗長な対象表現を省略し、状態異常は `[gold]効果名[/gold]{数値}を付与する`、詠唱・火種は `[gold]名称[/gold]{数値}を得る` へ統一。ブロック表記も本体の色付けに合わせた。
- **参考**：`docs/参考_Base/cards.json` の日本語説明（強打・ポンメルストライク・防御・アッパーカット等）。
- **表示修正**：炎の守りの説明文に含まれていた二重エスケープの改行を修正。
- **変更理由**：既存スレスパ2カードとの日本語表現の差を減らし、カードの読みやすさと用語の統一感を高めるため。
- **維持**：カード効果、動的変数名、カード名、英語ローカライズ、C#の処理は変更しない。長文・特殊なカードの効果説明は意味を変えない範囲に留めた。
- **確認状況**：GitHubへの反映とJSONの解析は確認。**ビルド・ゲーム内表示は未確認**。

## 2026-11-20：炎の守りを新規実装

- **追加**：`HinamoriModCode/Cards/FlameGuard.cs`。コモン、1EN、自分に8ブロックを付与して火種を5獲得。強化後は8ブロックのまま火種8獲得。日本語コメントで通常・強化後の効果、処理順、共通コマンドの利用目的を記載。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json` に炎の守りのカード名・効果を追加。前回追加した飛梅の一閃の説明文について、文字列内の改行が二重エスケープされていたため修正。
- **更新**：`docs/CardList.md` の炎の守りを `実装済・未確認` に変更。
- **更新**：`docs/ImplementedMethodReference.md` に `GainsBlock`、`BlockVar`、`CreatureCmd.GainBlock`、`ForgeVar`、`EmberCmd.GainEmber`、`UpgradeValueBy` と参照元を追加。
- **変更理由**：既存のブロック獲得・火種獲得処理を再利用し、設計表の飛梅・火種カードを実装するため。飛梅の一閃の改行表示を正しくするため。
- **確認状況**：GitHubへの反映完了。ローカライズJSONは解析可能な形式で生成。**ビルド・ゲーム内動作未確認**。

## 2026-11-20：飛梅の一閃を新規実装

- **追加**：`HinamoriModCode/Cards/TobiumesSlash.cs`。コモン、1EN、敵1体に7ダメージを与え、攻撃後に火種を7獲得。強化後はダメージ7のまま火種10獲得。日本語コメントで通常・強化後の効果、対象確認、攻撃と火種の処理順序を説明。
- **更新**：`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json` にカード名と効果説明を追加。
- **更新**：`docs/CardList.md` の「飛梅の一閃」を `実装済・未確認` に変更。
- **更新**：`docs/ImplementedMethodReference.md` に使用した `DamageCmd.Attack`、`EmberCmd.GainEmber`、`DynamicVars.Forge.UpgradeValueBy` 等と参照元を追記。
- **変更理由**：設計表の未実装カードを、既存の単体攻撃と火種獲得コマンドを再利用して追加するため。
- **確認状況**：GitHubへのコード・ローカライズ・ドキュメントの反映を確認。**ビルド・ゲーム内動作未確認**。

## 2026-11-20：全88ソース調査・メソッド索引・日本語コメント追加

- **追加資料**：`docs/SourceAuditPart1.md` ～ `SourceAuditPart6.md`、`docs/AllSourceMethodIndex.md`。
- **調査内容**：`HinamoriModCode` 配下のC# **88ファイル**をすべて取得。ドット付きメソッド呼び出し **229種類**を抽出し、呼び出し元・使用例の逆引き索引を作成。抽出は簡易解析であり、すべてのメソッド定義や動作を保証しない。
- **日本語コメント追加**：`Cards/Meditation.cs`、`Thrust.cs`、`BoundLightning.cs`、`Slash.cs`、`Shunpo.cs`、`ChantPreparation.cs`、`CrawlingRope.cs`、`FlamesPresence.cs`、`Commands/EmberCmd.cs`、`Powers/BurnPower.cs`。
- **コメント内容**：カード効果と強化差分、処理順序、対象判定、火種と飛梅の生成条件、火傷のダメージタイミング・減少タイミング。
- **動作変更**：なし（コメントとドキュメントのみ）。
- **確認状況**：GitHubソース本文の確認とコミットは完了。**ビルド・ゲーム内動作は未確認**。既存全88ファイルへのコメント追加が完了したという意味ではない。

# 雛森桃MOD 変更履歴

このファイルは、作業ブランチで実施した変更を利用者が確認するための記録です。

## 記録ルール

- コミットメッセージは**日本語**にする。
- 変更のたびに、変更対象ファイル・変更内容・変更理由・動作確認状況を記録する。
- C#の追加・修正時には、カードの役割・通常/強化後効果・主要な処理・特殊な判定理由を**日本語コメント**で説明する。
- 動作未確認のものを「動作確認済み」と記載しない。
- `Ver.1.0.0` には直接変更を加えず、作業ブランチで開発する。

## 2026-11-20：既存カードのメソッド一覧を作成しコメントを補強

- **追加**：`docs/ImplementedMethodReference.md`
- **内容**：直接確認した12カードと共通基底クラスの使用メソッド、処理の意味、実装例、注意点を一覧化。既存コードにないAPIを推測していない。全カードの網羅調査は未完了と明記。
- **コメント追加対象**：`HinamoriModCode/Cards/WhiteLightning.cs`、`SpiritualEnergyConversion.cs`、`BlueFirefall.cs`、`RedSmokeEscape.cs`、`SpiritualPressureFocus.cs`、`SpiritualPressureRelease.cs`、`ChainBinding.cs`。
- **変更理由**：新規カード実装時に既存処理を探し直す負担を減らし、既存ソースの処理意図を分かりやすくするため。
- **動作変更**：なし（コメントとドキュメントのみ）。
- **確認状況**：GitHubの実装ソースを確認して記録。ビルド・ゲーム内動作は未確認。今後、未調査カードの確認と一覧への追記が必要。

## 2026-11-20：カード一覧の実装状況表記を統一

- **変更対象**：`docs/CardList.md`
- **変更内容**：「逆境の炎」の実装状況を「実装済・未確認」に変更。
- **今後のルール**：カードのコードを実装した時点では、ゲーム内動作確認が済むまでカード一覧に必ず「実装済・未確認」と記載する。
- **確認状況**：表記変更のみ。ゲーム内動作は未確認。

## 2026-11-20：逆境の炎を実装（ゲーム内動作未確認）

- **変更対象**：`HinamoriModCode/Cards/AdversityFlame.cs`、`HinamoriMod/localization/jpn/cards.json`、`HinamoriMod/localization/eng/cards.json`、`docs/CardList.md`
- **変更内容**：アンコモン・1ENの「逆境の炎」を追加。通常は手札の状態異常をすべて廃棄し、廃棄札に移動した枚数×2の火種を獲得。強化後は呪いも対象に追加。
- **実装方法**：既存の「霊圧解放」と同じ `CardCmd.Exhaust` と、既存の「炎の気配」と同じ `EmberCmd.GainEmber` を使用。手札の対象を先に一覧化し、廃棄後に廃棄札に存在するカードを計数。火種は合計値を1回で獲得する。
- **コメント**：C#に通常・強化後効果、対象の絞り込み、廃棄成功判定、火種付与の理由を日本語で記載。
- **変更理由**：カード一覧の未実装カードを1枚追加し、既存APIの再利用で実装するため。
- **確認状況**：GitHubへコミット済み。**ビルド・ゲーム内動作は未確認**。特に `CardType.Status` / `CardType.Curse` の判定、廃棄不可カードの扱い、ローカライズ表示、火種獲得回数を要検証。
- **確認してほしい動作**：状態異常2枚なら火種＋4、強化前は呪いを残す、強化後は呪いも廃棄する、対象なしなら火種＋0。

## 2026-11-20：開発ルールの整備

- **変更対象**：`docs/CHANGELOG.md`
- **変更内容**：日本語コミット、日本語コメント、変更履歴の記録ルールを追加。
- **変更理由**：修正内容とコードの動作を追いやすくするため。
- **確認状況**：ドキュメント変更のみ。ゲーム内動作確認は不要。

## 2026-11-20：参考資料・確認事項の管理（過去の作業）

- **変更対象**：`docs/ImplementationReferenceChecklist.md`
- **変更内容**：実装待ちカードと追加資料の確認事項を整理。後続の修正で、`docs/参考_Base` は本体C#コードではなくカード一覧・Localization等の参考資料であることを明記。
- **変更理由**：既存の雛森MODコードを優先して再利用し、必要な追加資料を具体的に管理するため。
- **確認状況**：ドキュメント変更のみ。カード本体の追加実装は未着手。
- **補足**：この履歴の対象となる過去のコミットメッセージは英語でした。以後は日本語に統一します。
