
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「百歩欄干」
/// 1EN＋詠唱2
/// 敵1体に弱体2、自分に活力2。
/// 次のアタックのEN・詠唱コストを0にする。
/// 強化後：弱体3、活力3。
/// </summary>
public class HundredStepRailingsMomo : HinamoriModCard
{
    public override int ChantCost => 2;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(2m),
        new PowerVar<VigorPower>(2m)
    ];

    public HundredStepRailingsMomo()
        : base(
            1,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 敵に弱体を付与
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Vulnerable.BaseValue,
            Owner.Creature,
            this);

        // 自分に活力を付与
        await PowerCmd.Apply<VigorPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["VigorPower"].BaseValue,
            Owner.Creature,
            this);

        // 次のアタックを無料化
        await PowerCmd.Apply<FreeChantAttackPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
        DynamicVars["VigorPower"].UpgradeValueBy(1m);
    }
}
