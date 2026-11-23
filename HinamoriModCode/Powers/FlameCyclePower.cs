using MegaCrit.Sts2.Core.Entities.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>炎の循環（通常）：火種獲得時、ランダムな敵1体へ火傷。</summary>
public class FlameCyclePower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/FlameCyclePower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/FlameCyclePower.png";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
}
