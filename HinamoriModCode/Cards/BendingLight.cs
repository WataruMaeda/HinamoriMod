
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「曲光」
/// 0EN＋詠唱1、8ブロック
/// 強化後：11ブロック
/// </summary>
public class BendingLightMomo : HinamoriModCard
{
    // 詠唱コスト
    public override int ChantCost => 1;

    // 鬼道タグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    // ブロック獲得カード
    public override bool GainsBlock => true;

    // 基本ブロック
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move)
    ];

    public BendingLightMomo()
        : base(
            0,
            CardType.Skill,
            CardRarity.Common,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱を1消費
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // ブロックを獲得
        global::HinamoriMod.HinamoriModCode.Character.HinamoriCombatAnimation.Play("cast");

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        // ブロック8 → 11
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
