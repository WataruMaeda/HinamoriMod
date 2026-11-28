using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

public class TobiumeLingeringFlame : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner || !Owner.Creature.IsAlive)
            return;

        var combat = Owner.Creature.CombatState;
        if (combat == null)
            return;
        var enemies = combat.GetOpponentsOf(Owner.Creature).Where(x => x.IsAlive).ToList();
        if (enemies.Count == 0)
            return;
        var enemy = player.RunState.Rng.CombatCardGeneration.NextItem(enemies);
        await PowerCmd.Apply<BurnPower>(null, enemy, 1, Owner.Creature, null);
    }
}
