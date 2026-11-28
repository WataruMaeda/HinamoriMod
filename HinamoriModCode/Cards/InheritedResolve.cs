using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>受け継ぐ想い。全キャラプールとスターコストは未対応。</summary>
public class InheritedResolveMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<InheritedResolvePower>(1m)
    ];

    public InheritedResolveMomo()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        if (IsUpgraded)
            await PowerCmd.Apply<UpgradedInheritedResolvePower>(
                choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        else
            await PowerCmd.Apply<InheritedResolvePower>(
                choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // IsUpgraded に応じて適用するパワーを切り替える。
    }
}
