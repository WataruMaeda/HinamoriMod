using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>
/// 飛梅の鍔：戦闘開始時に火種を10獲得する。
/// </summary>
public class TobiumeGuard : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
            await EmberCmd.GainEmber(10m, Owner);
    }
}
