
using Godot;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.UI;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
public static class EmberCounterPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        NCombatUi __instance,
        CombatState state)
    {
        var player = LocalContext.GetMe(state);

        if (player.Character.GetType().Name != "HinamoriMod")
            return;

        if (__instance.GetNodeOrNull<NEmberCounter>(
                "HinamoriEmberCounter") != null)
            return;

        var starCounter =
            __instance.GetNodeOrNull<NStarCounter>("%StarCounter");

        if (starCounter == null)
            return;

        if (starCounter.GetParent() is not Control parent)
            return;

        var emberCounter = new NEmberCounter
        {
            Name = "HinamoriEmberCounter"
        };

        parent.AddChild(emberCounter);

        // 詠唱カウンターの真下に配置
        emberCounter.Position =
            starCounter.Position + new Vector2(0, 65);

        emberCounter.Initialize(player);
    }
}
