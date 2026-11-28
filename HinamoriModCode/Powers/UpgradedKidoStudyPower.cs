namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>強化済み鬼道の研鑽。生成したカードをアップグレードする。</summary>
public class UpgradedKidoStudyPower : KidoStudyPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/UpgradedKidoStudyPower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/UpgradedKidoStudyPower.png";

    protected override bool UpgradeGeneratedCards => true;
}
