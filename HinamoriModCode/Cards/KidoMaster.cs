
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 鬼道の達人
/// 2EN、毎ターン開始時に詠唱を2獲得する。
/// 強化後は3獲得する。
/// </summary>
public class KidoMasterMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<KidoMasterPower>(2m)
    ];

    public KidoMasterMomo()
        : base(
            2,
            CardType.Power,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        await PowerCmd.Apply<KidoMasterPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["KidoMasterPower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["KidoMasterPower"].UpgradeValueBy(1m);
    }
}
