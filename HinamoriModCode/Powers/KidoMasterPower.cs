
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 鬼道の達人
/// 毎ターン開始時、詠唱をAmount分獲得する。
/// </summary>
public class KidoMasterPower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/KidoMasterPower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/KidoMasterPower.png";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override bool IsVisibleInternal => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
            return;

        if (!Owner.IsAlive || Amount <= 0)
            return;

        await PowerCmd.Apply<ChantPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Amount,
            Owner,
            null);
    }
}
