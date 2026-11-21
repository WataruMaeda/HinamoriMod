using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 霊圧制御。鬼道を使用するたびに3ブロックを得る。
/// 複数枚使用した場合はスタック数に応じて獲得量が増える。
/// </summary>
public class SpiritualPressureControlPower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 自分が使用した鬼道カードのみ反応する。
        if (cardPlay.Card.Owner.Creature != Owner ||
            cardPlay.Card is not HinamoriModCard momoCard ||
            !momoCard.HasHinamoriTag(HinamoriCardTag.Kido) ||
            Amount <= 0)
            return;

        // カード1回につき3ブロック。複数設置時はAmountが増える。
        await CreatureCmd.GainBlock(Owner, new BlockVar(Amount, ValueProp.Unpowered), cardPlay);
    }
}
