# 全ソース調査 第3群（31〜45件）

> ドット付きメソッド呼び出しを抽出。ビルド・ゲーム内動作未確認。

| ファイル | クラス | 呼び出しメソッド | コメント行数 |
|---|---|---|---:|
| [`Restrain.cs`](../HinamoriModCode/Cards/Restrain.cs) | RestrainMomo | `ArgumentNullException.ThrowIfNull`、`CreatureCmd.GainBlock`、`DynamicVars.Weak.UpgradeValueBy`、`PowerCmd.Apply<WeakPower>` | 7 |
| [`RoundLockFan.cs`](../HinamoriModCode/Cards/RoundLockFan.cs) | RoundLockFanMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy` | 12 |
| [`Shunpo.cs`](../HinamoriModCode/Cards/Shunpo.cs) | ShunpoMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy` | 10 |
| [`SixRodLightPrison.cs`](../HinamoriModCode/Cards/SixRodLightPrison.cs) | SixRodLightPrisonMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy`、`PowerCmd.Apply<FreeChantPowerCardPower>`、`PowerCmd.Apply<WeakPower>` | 10 |
| [`Slash.cs`](../HinamoriModCode/Cards/Slash.cs) | SlashMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy` | 9 |
| [`SpiritualEnergyConversion.cs`](../HinamoriModCode/Cards/SpiritualEnergyConversion.cs) | SpiritualEnergyConversionMomo | `DynamicVars.Energy.UpgradeValueBy`、`PlayerCmd.GainEnergy` | 11 |
| [`SpiritualPressureFocus.cs`](../HinamoriModCode/Cards/SpiritualPressureFocus.cs) | SpiritualPressureFocusMomo | `CardPileCmd.Draw`、`DynamicVars.Cards.UpgradeValueBy`、`PowerCmd.Apply<ChantPower>` | 11 |
| [`SpiritualPressureRelease.cs`](../HinamoriModCode/Cards/SpiritualPressureRelease.cs) | SpiritualPressureReleaseMomo | `CardCmd.Exhaust`、`CardSelectCmd.FromHand`、`PileType.Hand.GetPile`、`PowerCmd.Apply<ChantPower>` | 16 |
| [`SplittingVoid.cs`](../HinamoriModCode/Cards/SplittingVoid.cs) | SplittingVoidMomo | `PowerCmd.Apply<BufferPower>` | 7 |
| [`Thrust.cs`](../HinamoriModCode/Cards/Thrust.cs) | ThrustMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy` | 5 |
| [`ThunderRoarCannon.cs`](../HinamoriModCode/Cards/ThunderRoarCannon.cs) | ThunderRoarCannonMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy` | 9 |
| [`Tobiume.cs`](../HinamoriModCode/Cards/Tobiume.cs) | TobiumeMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`EmberState.GetAmount`、`EnergyCost.UpgradeBy`、`Math.Max`、`PowerCmd.Apply<BurnPower>`、`base.AfterCloned`、`base.AfterCreated`、`decimal.Floor` | 14 |
| [`TracingSparrow.cs`](../HinamoriModCode/Cards/TracingSparrow.cs) | TracingSparrowMomo | `CardFactory.GetDistinctForCombat`、`CardPileCmd.AddGeneratedCardToCombat`、`CardSelectCmd.FromChooseACardScreen`、`momoCard.SetChantFreeThisTurn`、`selected.SetToFreeThisTurn` | 20 |
| [`TripleBeakStrike.cs`](../HinamoriModCode/Cards/TripleBeakStrike.cs) | TripleBeakStrikeMomo | `ArgumentNullException.ThrowIfNull`、`CardPileCmd.Draw`、`DynamicVars.Weak.UpgradeValueBy`、`PowerCmd.Apply<WeakPower>` | 8 |
| [`TwinLotusBlueFirefall.cs`](../HinamoriModCode/Cards/TwinLotusBlueFirefall.cs) | TwinLotusBlueFirefallMomo | `DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy`、`PowerCmd.Apply<BurnPower>` | 8 |
