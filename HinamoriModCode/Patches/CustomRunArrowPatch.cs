using System;
using System.Linq;
using BaseLib.BaseLibScenes;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;

namespace HinamoriMod.HinamoriModCode.Patches;

// Keep the existing CustomRunCharacterPatch and CustomRunScrollPatch.
[HarmonyPatch(typeof(NCustomRunScreen), "OnSubmenuOpened")]
public static class CustomRunArrowPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCustomRunScreen __instance)
    {
        try
        {
            var field = AccessTools.Field(typeof(NCustomRunScreen), "_charButtonContainer");
            var content = field?.GetValue(__instance) as Control;
            if (content == null) return;

            var characters = content.GetChildren().OfType<NCharacterSelectButton>().ToArray();
            if (characters.Length <= 6) return;

            var scroll = content.GetParent() as NHorizontalScrollContainer;
            if (scroll == null)
            {
                GD.PrintErr("[HinamoriMod] Arrow patch: scroll container not found.");
                return;
            }

            // Put arrows beside the list's top edge, outside the clipping viewport.
            var host = scroll.GetParent() as Control;
            if (host == null) return;

            const string leftName = "HinamoriCustomArrowLeft";
            const string rightName = "HinamoriCustomArrowRight";
            if (host.HasNode(leftName) || host.HasNode(rightName)) return;

            var left = MakeArrow(leftName, "◀");
            var right = MakeArrow(rightName, "▶");
            host.AddChild(left);
            host.AddChild(right);

            // Above the character icons, avoiding overlap with them.
            left.Position = scroll.Position + new Vector2(0, -40);
            right.Position = scroll.Position + new Vector2(Math.Max(0, scroll.Size.X - 36), -40);

            int currentIndex = 0;
            void UpdateArrows()
            {
                left.Disabled = currentIndex <= 0;
                right.Disabled = currentIndex >= characters.Length - 1;
            }

            for (int i = 0; i < characters.Length; i++)
            {
                int index = i;
                characters[i].FocusEntered += () =>
                {
                    currentIndex = index;
                    UpdateArrows();
                };
            }

            void Move(int delta)
            {
                currentIndex = Math.Clamp(currentIndex + delta, 0, characters.Length - 1);
                var target = characters[currentIndex];
                target.GrabFocus();
                target.Select();
                UpdateArrows();
            }

            left.Pressed += () => Move(-1);
            right.Pressed += () => Move(1);
            UpdateArrows();
            GD.Print($"[HinamoriMod] Custom run arrows installed. Characters={characters.Length}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[HinamoriMod] Custom run arrows failed: {ex}");
        }
    }

    private static Button MakeArrow(string name, string label)
    {
        return new Button
        {
            Name = name,
            Text = label,
            CustomMinimumSize = new Vector2(36, 32),
            Size = new Vector2(36, 32),
            FocusMode = Control.FocusModeEnum.None,
            MouseFilter = Control.MouseFilterEnum.Stop,
            TooltipText = label == "◀" ? "前のキャラクター" : "次のキャラクター"
        };
    }
}
