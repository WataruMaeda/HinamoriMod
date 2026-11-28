
using System;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using HinamoriMod.HinamoriModCode.Commands;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace HinamoriMod.HinamoriModCode.Debug;

internal static class DebugResourceHelper
{
    public static int Energy(Player player) =>
        player.PlayerCombatState.Energy;

    public static int Chant(Player player) =>
        player.Creature.GetPower<ChantPower>()?.Amount ?? 0;

    // プレイヤーの火種を直接取得
    public static decimal Ember(Player player) =>
        EmberState.GetAmount(player);

    public static async Task AdjustEnergy(Player player, int delta)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;

        int target = Math.Max(0, Energy(player) + delta);
        await PlayerCmd.SetEnergy(target, player);
    }

    public static async Task AdjustChant(Player player, int delta)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;

        var power = player.Creature.GetPower<ChantPower>();

        int current = power?.Amount ?? 0;
        int target = Math.Max(0, current + delta);

        if (target == current)
            return;

        if (power == null)
        {
            await PowerCmd.Apply<ChantPower>(
                null!,
                player.Creature,
                target,
                player.Creature,
                null);
        }
        else if (target == 0)
        {
            await PowerCmd.Remove(power);
        }
        else
        {
            await PowerCmd.ModifyAmount(
                null!,
                power,
                target - current,
                player.Creature,
                null);
        }
    }


    public static async Task AdjustEmber(
        Player player,
        int delta)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;

        if (delta == 0)
            return;

        if (delta > 0)
        {
            await EmberCmd.GainEmber(delta, player);
            return;
        }

        // 減算時もEmberStateがカード表示値を同期。
        EmberState.AddAmount(player, delta);
    }
}
