
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
/// 縛道「崩輪」
/// 敵1体に脱力を付与する。
/// 次に使用するスキルのEN・詠唱コストを0にする。
/// </summary>
public class DisintegratingCircleMomo : HinamoriModCard
{
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    public override int ChantCost => 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1m)
    ];

    public DisintegratingCircleMomo()
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

        // 敵に脱力を付与
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Weak.BaseValue,
            Owner.Creature,
            this);

        // 次のスキルのEN・詠唱コストを0にする
        await PowerCmd.Apply<FreeChantSkillPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 脱力1 → 2
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}
