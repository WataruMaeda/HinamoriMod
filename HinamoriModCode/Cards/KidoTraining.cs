
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 鬼道の修練
/// 通常：1EN、詠唱＋1、次ターン詠唱＋3
/// 強化：1EN、詠唱＋1、次ターン詠唱＋4
/// </summary>
public class KidoTrainingMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Chant", 1m),
        new PowerVar<NextTurnChantPower>(3m)
    ];

    public KidoTrainingMomo()
        : base(
            1,
            CardType.Skill,
            CardRarity.Common,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 使用時に詠唱＋1
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);

        // 次ターンに獲得する詠唱を予約
        await PowerCmd.Apply<NextTurnChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["NextTurnChantPower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 次ターンの詠唱＋3 → ＋4
        DynamicVars["NextTurnChantPower"].UpgradeValueBy(1m);
    }
}
