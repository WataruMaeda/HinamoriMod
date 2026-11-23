using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 飛梅と共に：各ターンのエナジーリセット後に火種を獲得する。
/// </summary>
public class TogetherWithTobiumePower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/TogetherWithTobiumePower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/TogetherWithTobiumePower.png";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || !Owner.IsAlive || Amount <= 0)
            return;

        await EmberCmd.GainEmber(Amount, player);
    }
}
