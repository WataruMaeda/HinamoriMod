# 全ソース調査 第6群（76〜88件）

> ドット付きメソッド呼び出しを抽出。ビルド・ゲーム内動作未確認。

| ファイル | クラス | 呼び出しメソッド | コメント行数 |
|---|---|---|---:|
| [`FreeChantAttack.cs`](../HinamoriModCode/Powers/FreeChantAttack.cs) | FreeChantAttackPower | `PowerCmd.Decrement`、`_eligiblePlays.Add`、`_eligiblePlays.Remove` | 4 |
| [`FreeChantPowerCardPower.cs`](../HinamoriModCode/Powers/FreeChantPowerCardPower.cs) | FreeChantPowerCardPower | `PowerCmd.Decrement`、`_eligiblePlays.Add`、`_eligiblePlays.Remove` | 4 |
| [`FreeChantSkillPower.cs`](../HinamoriModCode/Powers/FreeChantSkillPower.cs) | FreeChantSkillPower | `PowerCmd.Decrement`、`_eligiblePlays.Add`、`_eligiblePlays.Remove` | 8 |
| [`FreeKidoChantPower.cs`](../HinamoriModCode/Powers/FreeKidoChantPower.cs) | FreeKidoChantPower | `PowerCmd.Decrement`、`_eligiblePlays.Add`、`_eligiblePlays.Remove`、`momoCard.HasHinamoriTag` | 4 |
| [`FullIncantationPower.cs`](../HinamoriModCode/Powers/FullIncantationPower.cs) | FullIncantationPower | `PowerCmd.Remove`、`_eligiblePlays.Add`、`_eligiblePlays.Remove`、`hinamoriCard.HasHinamoriTag` | 0 |
| [`HadoChantDiscountPower.cs`](../HinamoriModCode/Powers/HadoChantDiscountPower.cs) | HadoChantDiscountPower |  | 0 |
| [`HinamoriModPower.cs`](../HinamoriModCode/Powers/HinamoriModPower.cs) | for,that,manually,HinamoriModPower | `Id.Entry.RemovePrefix` | 15 |
| [`KidoMasterPower.cs`](../HinamoriModCode/Powers/KidoMasterPower.cs) | KidoMasterPower | `PowerCmd.Apply<ChantPower>` | 4 |
| [`NextTurnChantPower.cs`](../HinamoriModCode/Powers/NextTurnChantPower.cs) | NextTurnChantPower | `PowerCmd.Apply<ChantPower>`、`PowerCmd.Remove` | 7 |
| [`HinamoriModRelic.cs`](../HinamoriModCode/Relics/HinamoriModRelic.cs) | for,that,manually,means,HinamoriModRelic | `Id.Entry.RemovePrefix` | 9 |
| [`KidoFundamentals.cs`](../HinamoriModCode/Relics/KidoFundamentals.cs) | KidoFundamentals | `GD.Print`、`PowerCmd.Apply<ChantPower>`、`rewards.Add` | 9 |
| [`NChantCounter.cs`](../HinamoriModCode/UI/NChantCounter.cs) | NChantCounter | `ResourceLoader.Load<Texture2D>`、`_label.AddThemeColorOverride`、`_label.AddThemeConstantOverride`、`_label.AddThemeFontSizeOverride`、`_player.Creature.GetPower<ChantPower>`、`amount.ToString` | 2 |
| [`NEmberCounter.cs`](../HinamoriModCode/UI/NEmberCounter.cs) | NEmberCounter | `EmberState.GetAmount`、`ResourceLoader.Load<Texture2D>`、`_label.AddThemeColorOverride`、`_label.AddThemeConstantOverride`、`_label.AddThemeFontSizeOverride`、`amount.ToString` | 3 |
