using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>
/// 鬼道の真髄：戦闘開始時に詠唱7、2ターン目以降の開始時に詠唱1。
/// エンシェント追加報酬は五番隊副官章が管理する。
/// </summary>
public class KidoEssence : KidoFundamentals
{
    // 鬼道の心得から置き換える上位初期レリック。通常のレア枠には出さない。
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override int StartingChant => 7;

    private int _energyResetsThisCombat;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
            _energyResetsThisCombat = 0;

        await base.AfterRoomEntered(room);
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner || !Owner.Creature.IsAlive)
            return;

        _energyResetsThisCombat++;
        if (_energyResetsThisCombat <= 1)
            return;

        await PowerCmd.Apply<ChantPower>(
            null,
            Owner.Creature,
            1,
            Owner.Creature,
            null);
    }
}
