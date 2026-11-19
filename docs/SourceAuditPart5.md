# 全ソース調査 第5群（61〜75件）

> ドット付きメソッド呼び出しを抽出。ビルド・ゲーム内動作未確認。

| ファイル | クラス | 呼び出しメソッド | コメント行数 |
|---|---|---|---:|
| [`MainFile.cs`](../HinamoriModCode/MainFile.cs) | MainFile | `Assembly.GetExecutingAssembly`、`Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly`、`harmony.PatchAll` | 3 |
| [`ChantCostDisplay.cs`](../HinamoriModCode/Patches/ChantCostDisplay.cs) | ChantCostDisplay | `base.Initialize` | 3 |
| [`ChantCostPatch.cs`](../HinamoriModCode/Patches/ChantCostPatch.cs) | ChantCostPatch | `__instance.AddChild`、`__instance.GetGlobalTransformWithCanvas`、`__instance.GetNodeOrNull<ChantCostDisplay>`、`__instance.GetNodeOrNull<Control>`、`display.Initialize`、`starLabel.GetGlobalTransformWithCanvas` | 8 |
| [`ChantCounterPatch.cs`](../HinamoriModCode/Patches/ChantCounterPatch.cs) | ChantCounterPatch | `LocalContext.GetMe`、`__instance.GetNodeOrNull<NChantCounter>`、`__instance.GetNodeOrNull<NStarCounter>`、`chantCounter.Initialize`、`parent.AddChild`、`player.Character.GetType`、`starCounter.GetParent` | 4 |
| [`CustomModeUnlockPatch.cs`](../HinamoriModCode/Patches/CustomModeUnlockPatch.cs) | CustomModeUnlockPatch |  | 0 |
| [`CustomRunArrowPatch.cs`](../HinamoriModCode/Patches/CustomRunArrowPatch.cs) | CustomRunArrowPatch | `AccessTools.Field`、`GD.Print`、`GD.PrintErr`、`Math.Clamp`、`Math.Max`、`content.GetChildren`、`content.GetParent`、`host.AddChild`、`host.HasNode`、`scroll.GetParent`、`target.GrabFocus`、`target.Select` | 3 |
| [`CustomRunCharacterPatch.cs`](../HinamoriModCode/Patches/CustomRunCharacterPatch.cs) | CustomRunCharacterPatch | `AccessTools.DeclaredMethod`、`AccessTools.Field`、`GD.Print`、`GD.PrintErr`、`ModelDb.Character<HinamoriCharacter>`、`button.Init`、`container.AddChildSafely`、`container.GetChildCount`、`container.GetChildren`、`current.GetPath`、`seedInput.GetPath` | 3 |
| [`CustomRunScrollPatch.cs`](../HinamoriModCode/Patches/CustomRunScrollPatch.cs) | CustomRunScrollPatch | `AccessTools.Field`、`GD.Print`、`GD.PrintErr`、`buttons.GetChildren`、`buttons.GetParent`、`scroll.InitFocusScrolling` | 1 |
| [`EmberCounterPatch.cs`](../HinamoriModCode/Patches/EmberCounterPatch.cs) | EmberCounterPatch | `LocalContext.GetMe`、`__instance.GetNodeOrNull<NEmberCounter>`、`__instance.GetNodeOrNull<NStarCounter>`、`emberCounter.Initialize`、`parent.AddChild`、`player.Character.GetType`、`starCounter.GetParent` | 1 |
| [`NeowRelicPatch.cs`](../HinamoriModCode/Patches/NeowRelicPatch.cs) | NeowRelicPatch | `AccessTools.DeclaredMethod`、`RelicCmd.Obtain`、`player.GetRelic<KidoFundamentals>` | 6 |
| [`ResourceCostDisplay.cs`](../HinamoriModCode/Patches/ResourceCostDisplay.cs) | ResourceCostDisplay | `ResourceLoader.Load<Texture2D>`、`_costLabel.AddThemeColorOverride`、`_costLabel.AddThemeConstantOverride`、`_costLabel.AddThemeFontSizeOverride`、`cost.ToString` | 1 |
| [`StartingRelicFallbackPatch.cs`](../HinamoriModCode/Patches/StartingRelicFallbackPatch.cs) | StartingRelicFallbackPatch | `RelicCmd.Obtain`、`player.GetRelic<KidoFundamentals>` | 4 |
| [`HinamoriModPotion.cs`](../HinamoriModCode/Potions/HinamoriModPotion.cs) | HinamoriModPotion | `Id.Entry.RemovePrefix` | 0 |
| [`BurnPower.cs`](../HinamoriModCode/Powers/BurnPower.cs) | BurnPower | `CreatureCmd.Damage`、`PowerCmd.Decrement`、`participants.Contains`、`props.IsPoweredAttack` | 22 |
| [`ChantPower.cs`](../HinamoriModCode/Powers/ChantPower.cs) | ChantPower |  | 1 |
