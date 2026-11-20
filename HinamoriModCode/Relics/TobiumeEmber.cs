using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using System.Collections.Generic;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>飛梅の火種：自分のターン終了時に火種1。</summary>
public class TobiumeEmber : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterSideTurnEnd(
        MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        foreach (var participant in participants)
        {
            if (participant != Owner.Creature || !Owner.Creature.IsAlive)
                continue;

            await EmberCmd.GainEmber(1m, Owner);
            break;
        }
    }
}
