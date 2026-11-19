
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 飛梅との対話
/// 通常：2EN、詠唱+2、火種+10、次ターンEN+1
/// 強化：2EN、詠唱+2、火種+10、次ターンEN+2
/// </summary>
public class CommunionWithTobiumeMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Chant", 2m),
        new DynamicVar("Ember", 10m),
        new DynamicVar("NextEnergy", 1m)
    ];

    public CommunionWithTobiumeMomo()
        : base(
            2,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱+2
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);

        // 火種+10
        await EmberCmd.GainEmber(
            DynamicVars["Ember"].BaseValue,
            Owner);

        // 次ターンのエナジー獲得（通常+1、強化後+2）
        await PowerCmd.Apply<EnergyNextTurnPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["NextEnergy"].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 次ターンEN：+1 → +2
        DynamicVars["NextEnergy"].UpgradeValueBy(1m);
    }
}
