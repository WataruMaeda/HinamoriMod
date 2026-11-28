using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 飛梅と共に：毎ターン開始時に火種を獲得する。
/// 通常5、強化後8。重複使用時は獲得量が加算される。
/// </summary>
public class TogetherWithTobiumeMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TogetherWithTobiumePower>(5m)
    ];

    public TogetherWithTobiumeMomo()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        await PowerCmd.Apply<TogetherWithTobiumePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["TogetherWithTobiumePower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["TogetherWithTobiumePower"].UpgradeValueBy(3m);
    }
}
