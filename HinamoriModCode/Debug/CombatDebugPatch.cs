using HarmonyLib;
using HinamoriMod.HinamoriModCode.Debug;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HinamoriMod.HinamoriModCode.Debug;

[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom._Ready))]
internal static class CombatDebugPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCombatRoom __instance)
    {
        var handler = new DebugInputHandler { Name = "HinamoriCardDebugger" };
        __instance.AddChild(handler);
    }
}
