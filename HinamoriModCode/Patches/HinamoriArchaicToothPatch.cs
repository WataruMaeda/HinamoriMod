using System.Collections.Generic;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HinamoriMod.HinamoriModCode.Patches;

/// <summary>
/// Extend the vanilla transcendence mapping without replacing its existing entries.
/// The getter creates a new dictionary each time, so extend each returned dictionary.
/// </summary>
[HarmonyPatch(typeof(ArchaicTooth), "get_TranscendenceUpgrades")]
public static class HinamoriArchaicToothPatch
{
    [HarmonyPostfix]
    private static void AddHinamoriUpgrade(Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<RedFrameCannonMomo>().Id] = ModelDb.Card<IttoKasoMomo>();
    }
}
