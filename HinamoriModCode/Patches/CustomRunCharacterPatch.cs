
using System;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;
using HinamoriCharacter =
    HinamoriMod.HinamoriModCode.Character.HinamoriMod;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch]
public static class CustomRunCharacterPatch
{
    [HarmonyTargetMethod]
    private static MethodBase TargetMethod()
    {
        var method = AccessTools.DeclaredMethod(
            typeof(NCustomRunScreen), "InitCharacterButtons");

        GD.Print($"[HinamoriMod] CustomRun target: {method}");

        if (method == null)
        {
            GD.PrintErr(
                "[HinamoriMod] InitCharacterButtons was not found. Check game version.");
        }

        return method!;
    }

    [HarmonyPostfix]
    private static void Postfix(NCustomRunScreen __instance)
    {
        GD.Print(
            "[HinamoriMod] InitCharacterButtons Postfix executed!");

        try
        {
            var containerField = AccessTools.Field(
                typeof(NCustomRunScreen), "_charButtonContainer");

            var container =
                containerField?.GetValue(__instance) as Control;

            if (container == null)
            {
                GD.PrintErr(
                    "[HinamoriMod] _charButtonContainer not found or null.");
                return;
            }

            GD.Print(
                $"[HinamoriMod] Existing button count: {container.GetChildCount()}");

            var character = ModelDb.Character<HinamoriCharacter>();

            // 二重追加防止
            if (container.GetChildren()
                .OfType<NCharacterSelectButton>()
                .Any(button => button.Character?.Id == character.Id))
            {
                GD.Print(
                    "[HinamoriMod] Hinamori button already exists.");
                return;
            }

            // ボタン生成
            var button = PreloadManager.Cache
                .GetScene(
                    "res://scenes/screens/char_select/char_select_button.tscn")
                .Instantiate<NCharacterSelectButton>(
                    PackedScene.GenEditState.Disabled);

            button.Name = character.Id.Entry + "_button";

            container.AddChildSafely(button);
            button.Init(character, __instance);

            GD.Print(
                $"[HinamoriMod] Button added: {button.Name}; total: {container.GetChildCount()}");

            // フォーカス設定
            var buttons = container.GetChildren()
                .OfType<NCharacterSelectButton>()
                .ToArray();

            var seedField = AccessTools.Field(
                typeof(NCustomRunScreen), "_seedInput");

            var seedInput =
                seedField?.GetValue(__instance) as Control;

            if (seedInput == null)
            {
                GD.PrintErr(
                    "[HinamoriMod] _seedInput not found; skipping vertical focus links.");
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                var current = buttons[i];

                current.FocusNeighborLeft =
                    (i > 0 ? buttons[i - 1] : current).GetPath();

                current.FocusNeighborRight =
                    (i < buttons.Length - 1 ? buttons[i + 1] : current)
                    .GetPath();

                if (seedInput != null)
                {
                    current.FocusNeighborTop = seedInput.GetPath();
                }

                current.FocusNeighborBottom = current.GetPath();
            }

            GD.Print(
                $"[HinamoriMod] Character setup completed: {character.Id.Entry}; buttons: {buttons.Length}");
        }
        catch (Exception ex)
        {
            GD.PrintErr(
                $"[HinamoriMod] Custom character patch failed: {ex}");
        }
    }
}
