
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 次の鬼道カードの詠唱コストを0にする。
/// ターンをまたいで持続する。
/// </summary>
public class FreeKidoChantPower : HinamoriModPower
{
    private readonly HashSet<CardPlay> _eligiblePlays = new();

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override bool IsVisibleInternal => true;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card is not HinamoriModCard momoCard ||
            !momoCard.HasHinamoriTag(HinamoriCardTag.Kido) ||
            momoCard.ChantCost < 0)
        {
            return Task.CompletedTask;
        }

        _eligiblePlays.Add(cardPlay);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!_eligiblePlays.Remove(cardPlay))
            return;

        await PowerCmd.Decrement(this);
    }
}
