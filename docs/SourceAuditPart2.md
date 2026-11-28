# 全ソース調査 第2群（16〜30件）

> ドット付きメソッド呼び出しを抽出。ビルド・ゲーム内動作未確認。

| ファイル | クラス | 呼び出しメソッド | コメント行数 |
|---|---|---|---:|
| [`FullIncantation.cs`](../HinamoriModCode/Cards/FullIncantation.cs) | FullIncantationMomo | `PowerCmd.Apply<ChantPower>`、`PowerCmd.Apply<FullIncantationPower>` | 0 |
| [`HangingStar.cs`](../HinamoriModCode/Cards/HangingStar.cs) | HangingStarMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy`、`base.GetResultLocationForCardPlay` | 15 |
| [`HinamoriCardTag.cs`](../HinamoriModCode/Cards/HinamoriCardTag.cs) |  |  | 0 |
| [`HinamoriModCard.cs`](../HinamoriModCode/Cards/HinamoriModCard.cs) | HinamoriModCard | `Id.Entry.RemovePrefix`、`Math.Max`、`Owner.Creature.GetPower<ChantPower>`、`Owner.Creature.GetPower<FreeChantAttackPower>`、`Owner.Creature.GetPower<FreeChantPowerCardPower>`、`Owner.Creature.GetPower<FreeChantSkillPower>`、`Owner.Creature.GetPower<FreeKidoChantPower>`、`PowerCmd.ModifyAmount`、`PowerCmd.Remove`、`base.AfterSideTurnEnd` | 19 |
| [`HundredStepRailings.cs`](../HinamoriModCode/Cards/HundredStepRailings.cs) | HundredStepRailingsMomo | `ArgumentNullException.ThrowIfNull`、`DynamicVars.Vulnerable.UpgradeValueBy`、`PowerCmd.Apply<FreeChantAttackPower>`、`PowerCmd.Apply<VigorPower>`、`PowerCmd.Apply<VulnerablePower>` | 10 |
| [`IncantationAbandonment.cs`](../HinamoriModCode/Cards/IncantationAbandonment.cs) | IncantationAbandonmentMomo | `EnergyCost.UpgradeBy`、`PowerCmd.Apply<FreeKidoChantPower>` | 5 |
| [`InvertedMountainCrystal.cs`](../HinamoriModCode/Cards/InvertedMountainCrystal.cs) | InvertedMountainCrystalMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy`、`PowerCmd.Apply<BlurPower>` | 10 |
| [`KidoMaster.cs`](../HinamoriModCode/Cards/KidoMaster.cs) | KidoMasterMomo | `PowerCmd.Apply<KidoMasterPower>` | 5 |
| [`KidoProdigy.cs`](../HinamoriModCode/Cards/KidoProdigy.cs) | KidoProdigyMomo | `CardCmd.Upgrade`、`CardSelectCmd.FromHandForUpgrade`、`PileType.Hand.GetPile`、`PowerCmd.Apply<ChantPower>` | 10 |
| [`KidoTraining.cs`](../HinamoriModCode/Cards/KidoTraining.cs) | KidoTrainingMomo | `PowerCmd.Apply<ChantPower>`、`PowerCmd.Apply<NextTurnChantPower>` | 8 |
| [`Meditation.cs`](../HinamoriModCode/Cards/Meditation.cs) | MeditationMomo | `PowerCmd.Apply<ChantPower>` | 3 |
| [`RagingStorm.cs`](../HinamoriModCode/Cards/RagingStorm.cs) | RagingStormMomo | `CardPileCmd.Draw`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy` | 10 |
| [`RedFrameCannon.cs`](../HinamoriModCode/Cards/RedFrameCannon.cs) | RedFrameCannonMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy`、`EmberCmd.GainEmber`、`PowerCmd.Apply<BurnPower>` | 7 |
| [`RedSmokeEscape.cs`](../HinamoriModCode/Cards/RedSmokeEscape.cs) | RedSmokeEscapeMomo | `CardPileCmd.Draw`、`CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy` | 10 |
| [`Repulse.cs`](../HinamoriModCode/Cards/Repulse.cs) | RepulseMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy`、`PowerCmd.Apply<ReflectPower>` | 9 |
