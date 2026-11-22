# ターンの開始と終了

## DLL で確認した開始順

1. `CombatManager.StartTurn` はクリーチャーの開始処理と `Hook.BeforeSideTurnStart` を実行する。
2. プレイヤー側の `SetupPlayerTurn` は条件に応じて `ResetEnergy` または `AddMaxEnergyToCurrent` を呼び、`Hook.AfterEnergyReset` を待つ。
3. `Hook.BeforeHandDraw` → `Hook.ModifyHandDraw` → `Hook.AfterModifyingHandDraw`。第1ターンには先天性などに応じた山札順・ドロー枚数の調整がある。
4. `CardPileCmd.Draw(..., fromHandDraw: true)` の完了後、`Hook.AfterPlayerTurnStart` を呼ぶ。ディスパッチは Early、通常、Late の順。各プレイヤーに対する処理なので、マルチプレイの全員共通バリアとしては使用しない。
5. `StartTurn` のプレイヤー側経路では、サイド開始後の処理、オーブ処理、`RunAutoPrePlayPhase` を経てアクション実行側を再開する。騒乱は自動プレイ用フックから起動する。

`SetupPlayerTurn` の IL_00eb、IL_016b、IL_03c1、IL_0445 が EN 更新後フック、ドロー前フック、ドロー、ドロー後フックに対応する。

## DLL で確認した終了順

- プレイヤー側は `EndPlayerTurnPhaseOneInternal` で `AfterAutoPostPlayPhaseEntered` → `BeforeSideTurnEnd` → `DoTurnEnd` → `BeforeFlush` の順。戦闘終了判定が途中にある。 `DoTurnEnd` はオーブと Ethereal、ターン終了時の手札効果を処理する。
- `EndPlayerTurnPhaseTwoInternal` は `FlushPlayerHand` 後に `AfterSideTurnEnd` を呼ぶ。その後、サイドを敵側へ切り替える経路がある。
- 敵側の `EndEnemyTurnInternal` は `BeforeSideTurnEnd` → 後処理 → `AfterSideTurnEnd`。戦闘継続時はプレイヤー側へ切り替わる。

MOD の初手効果は `AfterPlayerTurnStart` で所有者と `PlayerCombatState.TurnNumber == 1` を確認する。毎ターンの EN 追加は `AfterEnergyReset` を使う。例は [AbstractModel](../API/AbstractModel.md) と [PlayerCmd](../API/PlayerCmd.md)。

## 未確認

追加ターン、切断・再接続、同時選択での全分岐。
