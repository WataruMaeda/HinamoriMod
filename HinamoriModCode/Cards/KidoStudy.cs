using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>鬼道の研鑽。強化後は生成カードをアップグレードする。</summary>
public class KidoStudyMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<KidoStudyPower>(1m)
    ];

    public KidoStudyMomo()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        if (IsUpgraded)
            await PowerCmd.Apply<UpgradedKidoStudyPower>(
                choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        else
            await PowerCmd.Apply<KidoStudyPower>(
                choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // IsUpgraded に応じて使用時に付与するパワーを切り替える。
    }
}
