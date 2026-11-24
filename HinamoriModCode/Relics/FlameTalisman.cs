using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>炎の護符：戦闘開始時、敵全体に火傷3。</summary>
public class FlameTalisman : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        var combat = Owner.Creature.CombatState;
        if (combat == null)
            return;

        foreach (var enemy in combat.GetOpponentsOf(Owner.Creature).Where(x => x.IsAlive).ToList())
        {
            await PowerCmd.Apply<BurnPower>(null, enemy, 3, Owner.Creature, null);
        }
    }
}
