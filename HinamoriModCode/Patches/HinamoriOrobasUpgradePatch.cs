using HarmonyLib;
using HinamoriMod.HinamoriModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HinamoriMod.HinamoriModCode.Patches;

/// <summary>
/// Orobas upgrades Hinamori's starter relic instead of giving a Circlet.
/// Preserve the per-Act Ancient reward counter when replacing the relic.
/// </summary>
[HarmonyPatch]
public static class HinamoriOrobasUpgradePatch
{
    [HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
    [HarmonyPostfix]
    private static void UpgradeStarterRelic(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is KidoFundamentals && starterRelic is not KidoEssence)
            __result = ModelDb.Relic<KidoEssence>().ToMutable();
    }

    [HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Replace))]
    [HarmonyPrefix]
    private static void PreserveAncientRewardCounter(RelicModel original, RelicModel replace)
    {
        if (original is KidoFundamentals oldRelic && replace is KidoEssence upgraded)
        {
            upgraded.AncientBonusCount = oldRelic.AncientBonusCount;
            upgraded.HasReceivedNeowRelic = oldRelic.HasReceivedNeowRelic;
        }
    }
}
