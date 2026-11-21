using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 燃え盛る炎：毎ターン開始時に敵全体へ火傷を付与する。
/// 通常2、強化後3。
/// </summary>
public class BlazingTobiumeMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BlazingTobiumePower>(2m)
    ];

    public BlazingTobiumeMomo()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BlazingTobiumePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["BlazingTobiumePower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlazingTobiumePower"].UpgradeValueBy(1m);
    }
}
