using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 霊圧の解放：ターン開始時に追加エナジーを獲得する。
/// ターン開始時フックの動作順は要ビルド・ゲーム内確認。
/// </summary>
public class SpiritualPressureUnleashedPower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/SpiritualPressureUnleashedPower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/SpiritualPressureUnleashedPower.png";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || !Owner.IsAlive || Amount <= 0)
            return;

        await PlayerCmd.GainEnergy((int)Amount, player);
    }
}
