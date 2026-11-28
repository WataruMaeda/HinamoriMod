using HarmonyLib;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Patches;

/// <summary>
/// Capture Void Form's free-play eligibility immediately before the base game
/// spends card resources, so custom Chant follows the same payment decision.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class CaptureVoidFormResourcePaymentPatch
{
    private static void Prefix(CardModel __instance)
    {
        if (__instance is HinamoriModCard card)
            card.CaptureVoidFormBeforeResourcePayment();
    }
}
