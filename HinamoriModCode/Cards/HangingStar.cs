
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「吊星」
/// 0EN＋詠唱2、10ブロック、保留
/// 使用後、手札に戻る
/// 強化後：13ブロック
/// </summary>
public class HangingStarMomo : HinamoriModCard
{
    // 詠唱コスト
    public override int ChantCost => 2;

    // 鬼道タグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Kido;

    // ブロック獲得カード
    public override bool GainsBlock => true;

    // 保留
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];

    // 基本ブロック
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10m, ValueProp.Move)
    ];

    public HangingStarMomo()
        : base(
            0,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱を消費
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // ブロック獲得
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);
    }

    // 使用後、捨て札ではなく手札へ戻る
    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation location =
            base.GetResultLocationForCardPlay();

        if (location.pileType == PileType.Discard)
        {
            location.pileType = PileType.Hand;
        }

        return location;
    }

    protected override void OnUpgrade()
    {
        // ブロック10 → 13
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
