using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 鬼道の真髄：鬼道を3回使用するごとにカードを引く。
/// 使用回数はターンを跨いで保持し、戦闘ごとに新しいパワーで初期化する。
/// </summary>
public class EssenceOfKidoPower : HinamoriModPower
{
    private int _kidoUseCount;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 他キャラや鬼道以外のカードはカウントしない。
        if (cardPlay.Card.Owner.Creature != Owner ||
            cardPlay.Card is not HinamoriModCard momoCard ||
            !momoCard.HasHinamoriTag(HinamoriCardTag.Kido))
            return;

        // リプレイも使用1回として数える。3回ごとに余りを残す。
        _kidoUseCount++;
        if (_kidoUseCount < 3)
            return;

        _kidoUseCount -= 3;

        // 同じパワーを複数枚設置した場合、1回の発動で枚数分引く。
        if (Amount > 0)
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}
