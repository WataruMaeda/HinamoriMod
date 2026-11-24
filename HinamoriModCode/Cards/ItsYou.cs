using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// お前か！！：筋力を獲得し、カードを1枚引く。廃棄しない。
/// 通常：1EN、筋力+1、1ドロー。強化：筋力+2、1ドロー。
/// </summary>
public class ItsYouMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1m),
        new CardsVar(1)
    ];

    public ItsYouMomo()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Attack", 0f);

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["StrengthPower"].BaseValue,
            Owner.Creature,
            this);

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
