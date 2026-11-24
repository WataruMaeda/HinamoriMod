using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>鬼道の護符：破道・縛道を5回使用するたびブロック10。</summary>
public class KidoTalisman : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    private int _kidoPlaysThisCombat;

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
            _kidoPlaysThisCombat = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not HinamoriModCard card ||
            card.Owner != Owner || !Owner.Creature.IsAlive ||
            !(card.HasHinamoriTag(HinamoriCardTag.Hado) ||
              card.HasHinamoriTag(HinamoriCardTag.Bakudo)))
            return;

        if (++_kidoPlaysThisCombat < 5)
            return;

        _kidoPlaysThisCombat -= 5;
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature,
            new BlockVar(10m, ValueProp.Unpowered), cardPlay);
    }
}
