
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 次のターン開始時に詠唱を獲得する。
/// 複数回付与された場合は獲得量を合算する。
/// </summary>
public class NextTurnChantPower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        // 自分のターン開始時のみ発動
        if (player != Owner.Player)
            return;

        // 詠唱を獲得
        await PowerCmd.Apply<ChantPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Amount,
            Owner,
            null);

        // 効果を使い切ったので削除
        await PowerCmd.Remove(this);
    }
}
