using Godot;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.UI;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
public static class ChantCounterPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        NCombatUi __instance,
        CombatState state)
    {
        var player = LocalContext.GetMe(state);

        // 雛森以外では表示しない
        if (player.Character.GetType().Name != "HinamoriMod")
            return;

        // 既に生成されている場合は追加しない
        if (__instance.GetNodeOrNull<NChantCounter>(
                "HinamoriChantCounter") != null)
            return;

        var starCounter =
            __instance.GetNodeOrNull<NStarCounter>("%StarCounter");

        if (starCounter == null)
            return;

        // スターと同じ親（エナジーカウンター）を使用
        if (starCounter.GetParent() is not Control parent)
            return;

        var chantCounter = new NChantCounter
        {
            Name = "HinamoriChantCounter"
        };

        parent.AddChild(chantCounter);

        // スターの配置を基準にする
        chantCounter.Position = starCounter.Position;

        chantCounter.Initialize(player);
    }
}