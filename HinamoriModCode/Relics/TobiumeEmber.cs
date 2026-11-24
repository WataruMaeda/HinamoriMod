using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>飛梅の火種：毎ターン開始時に火種3。</summary>
public class TobiumeEmber : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner || !Owner.Creature.IsAlive)
            return;

        await EmberCmd.GainEmber(3m, Owner);
    }
}
