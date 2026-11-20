using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>鬼道の護符：詠唱10消費ごとにブロック10。</summary>
public class KidoTalisman : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    private int _spentChantThisCombat;

    // TODO: HinamoriModCard.SpendChantで実際に支払った詠唱を通知する。
    // TODO: 閾値到達時にBlockCmdを実行する。リプレイ時の二重計上を防ぐ。
    public int CountBlockTriggers(int chantSpent)
    {
        if (chantSpent <= 0) return 0;
        _spentChantThisCombat += chantSpent;
        int triggers = _spentChantThisCombat / 10;
        _spentChantThisCombat %= 10;
        return triggers;
    }

    public override System.Threading.Tasks.Task AfterRoomEntered(MegaCrit.Sts2.Core.Rooms.AbstractRoom room)
    {
        if (room is MegaCrit.Sts2.Core.Rooms.CombatRoom) _spentChantThisCombat = 0;
        return System.Threading.Tasks.Task.CompletedTask;
    }
}
