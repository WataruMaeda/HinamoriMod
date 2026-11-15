
using System.Runtime.CompilerServices;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Patches;

/// <summary>
/// 戦闘中に使用した破道の回数を記録する。
/// リプレイは同じカード使用として扱い、追加計上しない。
/// </summary>
internal static class HadoUsageCounter
{
    private sealed class Counter
    {
        public int Count;
    }

    private static readonly ConditionalWeakTable<object, Counter>
        Counters = new();

    public static int GetCount(object combatState)
    {
        return Counters.TryGetValue(combatState, out var counter)
            ? counter.Count
            : 0;
    }

    public static void Increment(object combatState)
    {
        Counters.GetOrCreateValue(combatState).Count++;
    }
}

[HarmonyPatch(
    typeof(CardModel),
    nameof(CardModel.OnPlayWrapper))]
internal static class HadoUsageCounterPatch
{
    [HarmonyPrefix]
    private static void Prefix(CardModel __instance)
    {
        if (__instance is not HinamoriModCard card)
            return;

        if (!card.HasHinamoriTag(HinamoriCardTag.Hado))
            return;

        var state = card.CombatState;

        if (state == null)
            return;

        HadoUsageCounter.Increment(state);
    }
}
