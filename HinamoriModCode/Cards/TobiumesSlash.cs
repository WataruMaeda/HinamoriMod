using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 飛梅の一閃：敵1体に7ダメージを与え、火種を7獲得するコモンアタック。
/// 強化後はダメージを変えず、獲得する火種だけを7から10に増やす。
/// </summary>
public class TobiumesSlashMomo : HinamoriModCard
{
    // 攻撃ダメージと火種獲得量は、表示と実際の処理で同じ動的変数を参照する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new ForgeVar(7)
    ];

    public TobiumesSlashMomo()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 敵1体を対象とするカードのため、対象が未指定なら攻撃しない。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // まず対象にダメージを与える。カード由来の補正と攻撃演出を適用する。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 攻撃処理の後に火種を獲得する。飛梅の生成判定は既存の共通コマンドに委譲する。
        await EmberCmd.GainEmber(DynamicVars.Forge.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        // 強化後はダメージ7のまま、火種獲得量を7から10に変更する。
        DynamicVars.Forge.UpgradeValueBy(3);
    }
}
