using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

public class KidoPrayerBeads : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner || !Owner.Creature.IsAlive)
            return;

        await PowerCmd.Apply<ChantPower>(null, Owner.Creature, 1, Owner.Creature, null);
    }
}
