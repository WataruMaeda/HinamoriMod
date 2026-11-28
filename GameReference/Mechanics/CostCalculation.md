# EN・スター・MOD 独自コスト

## DLL で確認した流れ

1. `PlayCardAction.ExecuteAction` が使用可否を確認し、`CardModel.SpendResources()` を待つ。
2. `SpendResources` は `EnergyCost.GetAmountToSpend()` と `GetStarCostWithModifiers()` を読む。条件に応じ、`Hook.ShouldPayExcessEnergyCostWithStars` による支払い分岐がある。続いて `SpendEnergy`、`SpendStars` を呼ぶ。
3. 固定 EN コストの `GetAmountToSpend` は `GetWithModifiers(CostModifiers.All)` を使い、支払いを 0 以上に丸める。X は所有者の現在 EN を参照する分岐を通る。
4. 戦闘用の EN 補正では `Hook.ModifyEnergyCostInCombat` が通常フック群を処理してから `TryModifyEnergyCostInCombatLate` 群を処理する。スター用の公開オーバーライドは `TryModifyStarCost`。補正フックでは適用時に bool の true と補正後の out 値を返す。
5. 支払い後の `ResourceInfo` が `OnPlayWrapper` から `CardPlay.Resources` に渡る。コストを 0 にした効果の出所はこのデータに含まれない。

## MOD 向けの設計例

雛森 MOD の詠唱は MOD 側の追加コスト。表示・使用可否と消費に同じ基本ルールを使う。通常プレイで EN 支払い時点の無料化が必要なら、その時点の判定を保存して `SpendChant` で使用する。実行回のガードには `IsFirstInSeries` を使う。虚無化の具体例は [VoidFormPower](../Powers/VoidFormPower.md) を参照。

## 未確認

複数 MOD が同じ late フックで競合するときの個別の組み合わせ。X コストに MOD 独自の無料化を追加する挙動。
