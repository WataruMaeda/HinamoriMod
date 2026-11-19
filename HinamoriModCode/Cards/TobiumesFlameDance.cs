using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 飛梅の炎舞：1ENで敵1体に6ダメージを与え、対象の火傷数と同量の火種を得るアンコモンアタック。
/// 強化後は9ダメージ。火種獲得量は対象の火傷数に連動したまま変わらない。
/// </summary>
public class TobiumesFlameDanceMomo : HinamoriModCard
{
    // ダメージ値は説明文と攻撃処理の両方で使用する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public TobiumesFlameDanceMomo()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 敵1体を対象とするカードのため、対象未指定は許可しない。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 攻撃で対象が倒れても獲得量が変わらないよう、攻撃前の火傷数を保持する。
        // 火傷を持たない対象は0として扱い、火種を獲得しない。
        int burnAmount = Math.Max(
            0,
            cardPlay.Target.GetPower<BurnPower>()?.Amount ?? 0);

        // まず敵1体にダメージを与える。カード由来の補正を既存の攻撃処理へ委譲する。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 攻撃前に記録した火傷数と同量の火種を獲得する。
        // 火種0のときの無処理や飛梅の生成判定は共通コマンドに委譲する。
        await EmberCmd.GainEmber(burnAmount, Owner);
    }

    protected override void OnUpgrade()
    {
        // 強化ではダメージのみ6→9。火種の獲得条件は変えない。
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
