namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>受け継ぐ想い＋：生成カードを強化する。</summary>
public class UpgradedInheritedResolvePower : InheritedResolvePower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/UpgradedInheritedResolvePower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/UpgradedInheritedResolvePower.png";

    protected override bool UpgradeGeneratedCards => true;
}
