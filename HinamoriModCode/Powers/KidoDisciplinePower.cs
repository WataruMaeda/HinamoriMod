using MegaCrit.Sts2.Core.Entities.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 鬼道の鍛錬による永続的な詠唱コスト軽減量。
/// カードの共通詠唱コスト計算から参照する。
/// </summary>
public class KidoDisciplinePower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
}
