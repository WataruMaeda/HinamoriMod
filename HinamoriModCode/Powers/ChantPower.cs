
using MegaCrit.Sts2.Core.Entities.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

public class ChantPower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // Powerアイコンを非表示
    protected override bool IsVisibleInternal => false;
}
