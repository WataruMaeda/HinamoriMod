
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HinamoriMod.HinamoriModCode.Commands;

public static class EmberState
{
    private sealed class CombatEmber
    {
        public Dictionary<Player, decimal> Amounts { get; } = new();
    }

    private static readonly ConditionalWeakTable<object, CombatEmber>
        States = new();

    public static decimal GetAmount(Player player)
    {
        var combatState = player.Creature.CombatState;

        if (combatState == null)
            return 0m;

        if (!States.TryGetValue(combatState, out var state))
            return 0m;

        return state.Amounts.TryGetValue(player, out var amount)
            ? amount
            : 0m;
    }

    /// <summary>
    /// 火種を指定した値に設定する。
    /// 0未満にはならない。
    /// </summary>

    public static decimal SetAmount(Player player, decimal amount)
    {
        var combatState = player.Creature.CombatState;

        if (combatState == null)
            return 0m;

        var state = States.GetOrCreateValue(combatState);

        decimal next = Math.Max(0m, amount);
        state.Amounts[player] = next;

        SyncCards(player);

        return next;
    }

    public static void SyncCards(Player player)
    {
        if (player.Creature.CombatState == null)
            return;

        foreach (TobiumeMomo card in player.PlayerCombatState
                     .AllCards
                     .OfType<TobiumeMomo>())
        {
            if (card.IsMutable)
                card.SyncEmber();
        }
    }


    /// <summary>
    /// 火種を加算・減算する。
    /// </summary>
    public static decimal AddAmount(Player player, decimal amount)
    {
        return SetAmount(player, GetAmount(player) + amount);
    }
}
