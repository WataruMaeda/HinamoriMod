using HarmonyLib;
using MegaCrit.Sts2.Core.Saves;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch(typeof(ProgressState))]
public static class CustomModeUnlockPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ProgressState.IsEpochObtained))]
    private static void IsEpochObtainedPostfix(
        string epochId,
        ref bool __result)
    {
        if (epochId == "CUSTOM_AND_SEEDS_EPOCH")
            __result = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ProgressState.IsEpochRevealed))]
    private static void IsEpochRevealedPostfix(
        string epochId,
        ref bool __result)
    {
        if (epochId == "CUSTOM_AND_SEEDS_EPOCH")
            __result = true;
    }
}