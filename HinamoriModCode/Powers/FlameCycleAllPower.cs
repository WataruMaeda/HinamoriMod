using MegaCrit.Sts2.Core.Entities.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>炎の循環（強化）：火種獲得時、敵全体へ火傷。</summary>
public class FlameCycleAllPower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/FlameCycleAllPower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/FlameCycleAllPower.png";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
}
