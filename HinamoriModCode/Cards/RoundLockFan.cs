
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「円閘扇」
/// 1EN＋詠唱2、18ブロック
/// 強化後：24ブロック
/// </summary>
public class RoundLockFanMomo : HinamoriModCard
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
        new BlockVar(18m, ValueProp.Move)
    ];

    public RoundLockFanMomo()
        : base(
            2,
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
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        // ブロック18 → 24
        DynamicVars.Block.UpgradeValueBy(6m);
    }
}
