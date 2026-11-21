using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Patches;

/// <summary>
/// Grants one additional relic from the current Ancient's own relic options
/// when an Ancient choice is made. The counter is saved on the starter relic.
/// </summary>
[HarmonyPatch]
public static class AncientBonusRelicPatch
{
    private static MethodBase TargetMethod() =>
        AccessTools.DeclaredMethod(typeof(AncientEventModel), "GenerateInitialOptionsWrapper");

    private static void Postfix(AncientEventModel __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__result == null)
            return;

        Godot.GD.Print("[HinamoriMod] Ancient bonus hook registered for " + __instance.GetType().Name);
        var displayedOptions = __result;
        foreach (EventOption option in displayedOptions)
        {
            if (option.IsLocked || option.IsProceed)
                continue;

            option.BeforeChosen += _ => GiveBonus(__instance, option, displayedOptions);
        }
    }

    private static async Task GiveBonus(AncientEventModel ancient, EventOption selectedOption, IReadOnlyList<EventOption> displayedOptions)
    {
        Godot.GD.Print("[HinamoriMod] Ancient bonus callback invoked for " + ancient.GetType().Name);
        var player = ancient.Owner;
        if (player == null)
            return;

        var starter = player.Relics.OfType<KidoFundamentals>().FirstOrDefault();
        if (starter == null)
        {
            Godot.GD.Print("[HinamoriMod] Ancient bonus skipped: starter relic missing");
            return;
        }

        int currentAct = player.RunState.CurrentActIndex;
        if (starter.AncientBonusCount > currentAct)
        {
            Godot.GD.Print("[HinamoriMod] Ancient bonus skipped: already granted for act " + currentAct);
            return;
        }

        var ownedIds = player.Relics.Select(r => r.Id).ToHashSet();
        if (selectedOption.Relic != null)
            ownedIds.Add(selectedOption.Relic.Id);

        // AllPossibleOptions is the Ancient's own set of relic-bearing options.
        // Never pull from the generic relic reward pool.
        List<RelicModel> candidates = ancient.AllPossibleOptions
            .Where(o => o.Relic != null)
            .Select(o => o.Relic!)
            .Where(r => r.Rarity == MegaCrit.Sts2.Core.Entities.Relics.RelicRarity.Ancient)
            .Where(r => !ownedIds.Contains(r.Id) && r.IsAllowed(player.RunState))
            .DistinctBy(r => r.Id)
            .ToList();

        // Some Ancients expose relics only in their generated visible options.
        if (candidates.Count == 0)
        {
            candidates = displayedOptions
                .Where(o => o.Relic != null)
                .Select(o => o.Relic!)
                .Where(r => r.Rarity == MegaCrit.Sts2.Core.Entities.Relics.RelicRarity.Ancient)
                .Where(r => !ownedIds.Contains(r.Id) && r.IsAllowed(player.RunState))
                .DistinctBy(r => r.Id)
                .ToList();
        }

        if (candidates.Count == 0)
        {
            Godot.GD.Print("[HinamoriMod] Ancient bonus skipped: no eligible Ancient relics for " + ancient.GetType().Name);
            return;
        }

        RelicModel bonus = candidates[Random.Shared.Next(candidates.Count)];
        Godot.GD.Print("[HinamoriMod] Ancient bonus selected: " + bonus.Id);
        await RelicCmd.Obtain(bonus, player);
        starter.AncientBonusCount = currentAct + 1;
        starter.Flash();
        Godot.GD.Print("[HinamoriMod] Ancient bonus granted: " + bonus.Id);
    }
}
