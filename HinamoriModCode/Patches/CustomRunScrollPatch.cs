
using System;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;
using BaseLib.BaseLibScenes;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch(typeof(NCustomRunScreen), "_Ready")]
public static class CustomRunScrollPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCustomRunScreen __instance)
    {
        try
        {
            var field = AccessTools.Field(
                typeof(NCustomRunScreen),
                "_charButtonContainer");

            var buttons = field?.GetValue(__instance) as Control;

            if (buttons == null)
            {
                GD.PrintErr(
                    "[HinamoriMod] Button container not found.");
                return;
            }

            var count = buttons.GetChildren()
                .OfType<NCharacterSelectButton>()
                .Count();

            if (count <= 6)
                return;

            var scroll = buttons.GetParent()
                as NHorizontalScrollContainer;

            if (scroll == null)
            {
                GD.PrintErr(
                    "[HinamoriMod] Scroll container not found.");
                return;
            }

            // BaseLibのフォーカススクロールを初期化
            scroll.InitFocusScrolling();

            GD.Print(
                $"[HinamoriMod] CustomRun scroll configured. " +
                $"Buttons={count}, " +
                $"ContentWidth={buttons.Size.X}, " +
                $"ViewportWidth={scroll.Size.X}");
        }
        catch (Exception ex)
        {
            GD.PrintErr(
                $"[HinamoriMod] CustomRun scroll failed: {ex}");
        }
    }
}
