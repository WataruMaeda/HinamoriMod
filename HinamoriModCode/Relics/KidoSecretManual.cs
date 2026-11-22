using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>鬼道の秘伝書：各ターン最初の破道・縛道の詠唱コストを0にする。</summary>
public class KidoSecretManual : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    private bool _usedThisTurn;

    // TODO: リプレイや詠唱0の鬼道での消費判定を検証する。
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom) ResetForTurn();
        return Task.CompletedTask;
    }

    public override Task AfterEnergyReset(Player player)
    {
        if (player == Owner) ResetForTurn();
        return Task.CompletedTask;
    }

    public bool IsDiscountAvailable => !_usedThisTurn;
    public void ResetForTurn() => _usedThisTurn = false;
    public void MarkKidoPlayed() => _usedThisTurn = true;
}
