
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 次のアタックのEN・詠唱コストを0にする。
/// 使用するまでターンをまたいで持続。
/// </summary>
public class FreeChantAttackPower : HinamoriModPower
{
    private readonly HashSet<CardPlay> _eligiblePlays = new();

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override bool IsVisibleInternal => true;

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card.Owner.Creature != Owner ||
            card.Type != CardType.Attack)
            return false;

        if (card.Pile?.Type != PileType.Hand &&
            card.Pile?.Type != PileType.Play)
            return false;

        modifiedCost = 0m;
        return true;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner ||
            cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

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
