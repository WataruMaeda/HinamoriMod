# HinamoriMod — AI 開発エージェント向け手順書

このファイルは、雛森桃をモチーフにした **Slay the Spire 2 MOD** の開発を、新しい会話・別のAIエージェントでも安全に引き継ぐための入口です。ユーザーが「GitHubで作業して」「続きから」と依頼したら、**作業前にこのファイルを読み、以下の順序でリポジトリの現状を確認**してください。過去の会話の記憶より、指定ブランチの実ファイルを優先します。

## 1. 対象とブランチ保護

- Repository: `WataruMaeda/HinamoriMod`
- 現在の継続開発ブランチ（本書作成時）: `feature/ongoing-fixes`
- `Ver.1.0.0` は基準版。**ユーザーの明示的な依頼なく変更・マージ・リベース・削除しない**。
- 作業開始時にブランチ一覧・最新コミット・対象ファイルを確認する。新しい会話でブランチ指定がなければ、原則 `feature/ongoing-fixes` を候補として確認し、勝手に別ブランチへ書き込まない。
- ユーザーの指示がない限り、関係ないカード・レリック・ローカライズを変更しない。コミットは変更内容が分かる単位にする。
- 既存の仕様が不明な場合、推測で上書きせず、現行実装・資料を確認し、それでも不明ならユーザーに確認する。

## 2. 新しいチャットでの必須確認順序

1. **本書 `AGENTS.md`** を読む。
2. **`README.md`** でプロジェクト構造と資料の入口を確認する。
3. **`GameReference/README.md`** を読み、調査済みAPI・処理フローの索引を把握する。
4. **`GameReference/GameVersion.md`** で解析した `sts2.dll` のビルド番号・SHA-256・解析日時を確認する。現行ゲームDLLとの一致は別途検証が必要。記載されたバージョンでの静的解析結果を最新版の保証として扱わない。
5. 依頼内容に応じ、**`GameReference/API/`、`GameReference/Mechanics/`、`GameReference/Powers/`** の該当ファイルを読む。全ファイルを無条件に読み込む必要はない。
6. **指定ブランチの実コード**（例: `HinamoriModCode/Cards/`、`Powers/`、`Relics/`、`Patches/`、`MainFile.cs`）と、必要なローカライズ（`HinamoriMod/localization/`）を確認する。
7. 変更前に影響範囲・既存の無料化、リプレイ、自動プレイ、詠唱・火種・火傷との相互作用を確認する。
8. 実装後、可能な範囲でビルド・テストを行い、未実施の検証は明確に未実施と報告する。GitHub上のソース閲覧だけで「ビルド成功」「ゲーム内動作確認済み」と言わない。

**GitHub検索の注意:** コード検索は既定ブランチのみを対象にする場合がある。作業ブランチの正確な内容は、必ずそのブランチを指定したファイル取得で確認する。

## 3. ゲーム本体の仕様を調べるとき

- **一次資料:** `GameReference/` は、ローカルの `sts2.dll` を Mono.Cecil で解析して作成した資料。資料中の「DLLで確認した」「MOD向け使用例」「未確認」を区別する。
- 参照優先順位: 対象ブランチの実コード → `GameReference` の該当解析 → 必要ならローカルの現行DLLで追加解析 → ユーザーへの確認。
- API名、引数、戻り値、フックの実行順、`CardPlay` のフラグを憶測で決めない。記載のないAPIは未確認として扱う。
- ローカルDLLが必要な追加調査は、アクセスできない環境ならユーザーに **具体的な解析対象だけ** 依頼する。既存資料に答えがある調査を繰り返させない。
- ゲームDLL本体・逆コンパイルしたソース一式・ゲームアセットをリポジトリに追加しない。APIシグネチャと独自の解析要約を保存する。
- ゲームアップデート時は `GameVersion.md` の SHA-256 と現行DLLを比較し、変化があれば影響する資料を再解析・更新する。

## 4. 実装上の重要な既存仕様

### 詠唱・カード使用
- 基本処理: `HinamoriModCode/Cards/HinamoriModCard.cs` の `EffectiveChantCost`、`IsPlayable`、`SpendChant` を確認する。
- リプレイでは詠唱を再消費しない（`CardPlay.IsFirstInSeries`）。
- 騒乱（Mayhem）などの自動プレイは `CardPlay.IsAutoPlay` が true なら通常の詠唱消費を省略する。
- **虚無化（Void Form）** は手動プレイのEN・スター無料化であり、`IsAutoPlay` は false。詠唱無料化は `VoidFormPower.TryModifyEnergyCostInCombatLate` の適用判定を用い、`CardModel.SpendResources` 直前に Harmony Prefix で記録する。参照: `GameReference/Powers/VoidFormPower.md`、`GameReference/Mechanics/CostCalculation.md`、`HinamoriModCode/Patches/CaptureVoidFormResourcePaymentPatch.cs`。
- **黒棺**（`BlackCoffinMomo`）は固定詠唱コストを持たず、独自に現在の詠唱を全消費し、リプレイでは初回の消費量を再利用する。一般的な無料化の変更でこの仕様を暗黙に変更しない。
- Xコストの虚無化対応は上記実装で対象外。根拠なしに対応済みと主張しない。

### 火種・火傷
- 火種（Ember）は戦闘中に蓄積する設計。火種獲得や参照の処理は現在のコードを優先して確認する。
- 火傷（BurnPower）は対象のターン開始時にスタック数に等しいHPダメージ、対象のターン終了時にスタックを1減らす。火傷中の攻撃ダメージは25%減（脱力と乗算）。変更時は `HinamoriModCode/Powers/BurnPower.cs` の実装を確認する。
- 鬼道の独自コスト、無料化、火種・火傷、複数ヒット、全体攻撃、リプレイ、ターン開始・終了処理は互いに影響するため、単独のカードだけでなく関連フックも確認する。

### レリック・パッチ
- レリック実装は `HinamoriModCode/Relics/`、Harmonyパッチは `HinamoriModCode/Patches/` を参照する。
- `HinamoriModCode/MainFile.cs` の `Initialize()` が `harmony.PatchAll(assembly)` を実行する。
- Harmonyで本体メソッドを変更する場合、DLLで正確な対象シグネチャとタイミングを確認する。マルチプレイやゲーム更新時の影響にも注意する。

## 5. 作業と検証の基本手順

1. ユーザーの要求を仕様として整理し、既存実装と矛盾がないか調べる。
2. 該当する `GameReference` を確認し、ゲーム本体のフック・呼び出し順を検証する。
3. 変更対象の全ファイルを **指定ブランチから最新取得** する。
4. 最小限の変更を行い、既存カード・パワー・レリックの効果を維持する。
5. 可能なら `dotnet build` と、関連するゲーム内シナリオを確認する。実行できなければ未検証と記載する。
6. GitHubにプッシュしたら **ブランチ、変更ファイル、コミット、確認済み事項、未確認事項** を報告する。
7. 新しいAPIや処理の解析が増えた場合、`GameReference` に再利用できる要約を追加する。ただしゲーム本体コードの大量転載は避ける。

## 6. よくある不具合の確認観点

- **CSエラー:** 対象クラスの名前空間、using、APIシグネチャ、型、非同期戻り値をDLL資料と照合する。
- **コストが意図せず消費される:** 表示・使用可否・実際の支払いの3経路を別々に確認する。自動プレイ、手動無料化、リプレイ、ポーションの一時無料化を混同しない。
- **効果が発動しない:** ターン開始・終了フックの順序、`Owner`、`CombatState`、対象プレイヤー、フック登録を確認する。
- **ダメージが違う:** `GameReference/Mechanics/DamageCalculation.md` と、火傷・脱力・ダメージ倍率・攻撃回数を確認する。
- **カード生成・ドロー:** `GameReference/Mechanics/CardGeneration.md`、`CardFactory`、`CardPileCmd`、`CardSelectCmd` を参照する。
- **画像・アニメーション:** Godotシーン、リソース参照、カード・キャラの表示タイミングを確認し、画像やアニメーションの変更はユーザーの指示範囲に限定する。

## 7. 新しい会話の開始例

ユーザーは次の一文で依頼を始められる:

> GitHubの `WataruMaeda/HinamoriMod` のルートにある `AGENTS.md` を最初に読み、`feature/ongoing-fixes` と `GameReference` を確認してから、雛森MODの開発を続けてください。

この文は入口の案内であり、AIがリポジトリを自動的に読むことを保証するものではない。新しいチャットでは、実際にGitHub接続と対象ファイルの取得ができているか確認する。
