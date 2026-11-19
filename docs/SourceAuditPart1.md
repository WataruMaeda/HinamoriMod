# 全ソース調査 第1群（1〜15件）

> 対象：作業ブランチのC#実装。ドット付きメソッド呼び出しを抽出。ビルド・ゲーム内動作未確認。

| ファイル | クラス | 呼び出しメソッド | コメント行数 |
|---|---|---|---:|
| [`AbolishingFlames.cs`](../HinamoriModCode/Cards/AbolishingFlames.cs) | AbolishingFlamesMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy`、`PowerCmd.Apply<BurnPower>` | 9 |
| [`AdversityFlame.cs`](../HinamoriModCode/Cards/AdversityFlame.cs) | AdversityFlameMomo | `CardCmd.Exhaust`、`Cards.Contains`、`EmberCmd.GainEmber`、`PileType.Exhaust.GetPile`、`PileType.Hand.GetPile` | 17 |
| [`AvengeCaptainAizen.cs`](../HinamoriModCode/Cards/AvengeCaptainAizen.cs) | AvengeCaptainAizenMomo | `ArgumentNullException.ThrowIfNull`、`CreatureCmd.GainBlock`、`DamageCmd.Attack`、`PowerCmd.Apply<BurnPower>`、`deckCard.IncreaseFromPlay` | 15 |
| [`BendingLight.cs`](../HinamoriModCode/Cards/BendingLight.cs) | BendingLightMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy` | 12 |
| [`BlackCoffin.cs`](../HinamoriModCode/Cards/BlackCoffin.cs) | BlackCoffinMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy`、`Math.Max`、`Owner.Creature.GetPower<ChantPower>`、`PowerCmd.ModifyAmount` | 10 |
| [`BlueFirefall.cs`](../HinamoriModCode/Cards/BlueFirefall.cs) | BlueFirefallMomo | `DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy`、`DynamicVars.Forge.UpgradeValueBy`、`EmberCmd.GainEmber` | 12 |
| [`BoundLightning.cs`](../HinamoriModCode/Cards/BoundLightning.cs) | BoundLightningMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy` | 4 |
| [`ChainBinding.cs`](../HinamoriModCode/Cards/ChainBinding.cs) | ChainBindingMomo | `CardPileCmd.Add`、`CardSelectCmd.FromChooseACardScreen`、`EnergyCost.UpgradeBy`、`PileType.Exhaust.GetPile`、`selected.AddKeyword`、`selected.RemoveKeyword` | 15 |
| [`ChantPreparation.cs`](../HinamoriModCode/Cards/ChantPreparation.cs) | ChantPreparationMomo | `PowerCmd.Apply<ChantPower>` | 9 |
| [`CommunionWithTobiume.cs`](../HinamoriModCode/Cards/CommunionWithTobiume.cs) | CommunionWithTobiumeMomo | `EmberCmd.GainEmber`、`PowerCmd.Apply<ChantPower>`、`PowerCmd.Apply<EnergyNextTurnPower>` | 9 |
| [`CrawlingFlame.cs`](../HinamoriModCode/Cards/CrawlingFlame.cs) | CrawlingFlameMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy`、`PowerCmd.Apply<BurnPower>` | 6 |
| [`CrawlingRope.cs`](../HinamoriModCode/Cards/CrawlingRope.cs) | CrawlingRopeMomo | `CreatureCmd.GainBlock`、`DynamicVars.Block.UpgradeValueBy`、`PowerCmd.Apply<ChantPower>` | 9 |
| [`DisintegratingCircle.cs`](../HinamoriModCode/Cards/DisintegratingCircle.cs) | DisintegratingCircleMomo | `ArgumentNullException.ThrowIfNull`、`DynamicVars.Weak.UpgradeValueBy`、`PowerCmd.Apply<FreeChantSkillPower>`、`PowerCmd.Apply<WeakPower>` | 8 |
| [`FlamesPresence.cs`](../HinamoriModCode/Cards/FlamesPresence.cs) | FlamesPresenceMomo | `EmberCmd.GainEmber` | 9 |
| [`FlyingDragonStrikingHeavenShakingThunderCannon.cs`](../HinamoriModCode/Cards/FlyingDragonStrikingHeavenShakingThunderCannon.cs) | FlyingDragonStrikingHeavenShakingThunderCannonMomo | `ArgumentNullException.ThrowIfNull`、`DamageCmd.Attack`、`DynamicVars.Damage.UpgradeValueBy`、`card.HasHinamoriTag`、`flyingDragon.GetHadoPlayCount` | 19 |
