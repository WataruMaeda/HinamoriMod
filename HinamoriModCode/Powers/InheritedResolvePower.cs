using MegaCrit.Sts2.Core.Entities.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 受け継ぐ想いの状態保持用。
/// 全キャラクターのパワーカード取得とスターコスト無料化が未確認のため、
/// 現時点ではカード生成効果は発生しない。
/// </summary>
public class InheritedResolvePower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
}
