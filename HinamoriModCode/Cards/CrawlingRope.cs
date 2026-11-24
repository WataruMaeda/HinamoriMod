
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「這縄」
/// 1EN、8ブロック、詠唱+1
/// 強化後：10ブロック、詠唱+2
/// </summary>
public class CrawlingRopeMomo : HinamoriModCard
{
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    // ブロックを獲得するスキルであることをゲーム本体に通知する。
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new DynamicVar("Chant", 1m)
    ];

    public CrawlingRopeMomo()
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
        // 先にブロックを付与する。BlockVarに各種ブロック補正を反映する。
        HinamoriMod.HinamoriModCode.Character.HinamoriCombatAnimation.Play("cast");

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        // 続いて自分に詠唱パワーを付与する。強化後は獲得量が1→2。
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // ブロック8 → 10
        DynamicVars.Block.UpgradeValueBy(2m);

        // 詠唱1 → 2
        DynamicVars["Chant"].UpgradeValueBy(1m);
    }
}
