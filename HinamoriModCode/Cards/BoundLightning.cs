
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 綴雷電
/// 5ダメージ*2
/// </summary>
public class BoundLightningMomo : HinamoriModCard
{
    // 破道・鬼道の両方の独自タグを持つ。
    public override HinamoriCardTag HinamoriTags =>
    HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    // 1ヒットのダメージとヒット回数を別々に管理する。
    // 強化後も2ヒットのままで、1ヒットあたりのダメージのみ増える。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new HitCountVar(2)
    ];

    public BoundLightningMomo()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 単体攻撃のため対象が必須。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // WithHitCountで2回攻撃にする。各ヒットの基礎値はDamageVarを使う。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.HitCount.IntValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 強化後は5×2→7×2ダメージ。
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
