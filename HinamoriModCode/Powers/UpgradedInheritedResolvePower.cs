namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>受け継ぐ想い＋：生成カードを強化する。</summary>
public class UpgradedInheritedResolvePower : InheritedResolvePower
{
    protected override bool UpgradeGeneratedCards => true;
}
