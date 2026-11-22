using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>鬼道の探究：ターン開始時に火種1を消費し、筋力と敏捷性を得る。</summary>
public class KidoResearchMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<KidoResearchPower>(1m)
    ];

    public KidoResearchMomo()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<KidoResearchPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);

        if (IsUpgraded)
            Owner.Creature.GetPower<KidoResearchPower>()?.AddUpgradedStack();
    }

    protected override void OnUpgrade()
    {
        // 強化時は毎ターン火種2消費、筋力・敏捷性+2に変更する。
    }
}
