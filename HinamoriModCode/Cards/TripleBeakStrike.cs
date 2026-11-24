
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「嘴突三閃」
/// 2EN、敵1体に脱力2、2ドロー
/// 強化後：脱力3、2ドロー
/// </summary>
public class TripleBeakStrikeMomo : HinamoriModCard
{
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2m),
        new CardsVar(2)
    ];

    public TripleBeakStrikeMomo()
        : base(
            2,
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

        // 敵1体に脱力を付与
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Weak.BaseValue,
            Owner.Creature,
            this);

        // カードを2枚引く
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        // 脱力2 → 3
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}
