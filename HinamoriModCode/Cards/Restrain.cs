
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「塞」
/// 0EN、3ブロック、敵1体に脱力1
/// 強化後：脱力2
/// </summary>
public class RestrainMomo : HinamoriModCard
{
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3m, ValueProp.Move),
        new PowerVar<WeakPower>(1m)
    ];

    public RestrainMomo()
        : base(
            0,
            CardType.Skill,
            CardRarity.Common,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // ブロックを獲得
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        // 敵1体に脱力を付与
        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Weak.BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}
