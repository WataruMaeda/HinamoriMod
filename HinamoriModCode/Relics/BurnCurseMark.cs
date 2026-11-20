using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>火傷の呪印：火傷状態の敵から受ける攻撃ダメージを軽減。</summary>
public class BurnCurseMark : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    // TODO: 被ダメージの乗算補正フックに接続し、攻撃者のBurnPowerを確認する。
    // TODO: 追加20%軽減は暫定値。最終数値を確認する。
    public static decimal DamageMultiplierWhenBurned => 0.8m;
}
