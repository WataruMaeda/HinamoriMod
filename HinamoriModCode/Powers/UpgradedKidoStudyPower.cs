namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>強化済み鬼道の研鑽。生成したカードをアップグレードする。</summary>
public class UpgradedKidoStudyPower : KidoStudyPower
{
    protected override bool UpgradeGeneratedCards => true;
}
