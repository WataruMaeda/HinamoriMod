
using System.Collections.Generic;
using HinamoriMod.HinamoriModCode.Cards;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 次に使用するパワーカードのEN・詠唱コストを0にする。
/// ターンをまたいで持続する。
/// </summary>
public class FreeChantPowerCardPower : HinamoriModPower
{
    // ゲーム本体の無料化パワー画像を参照する。
    public override string CustomPackedIconPath => "res://images/powers/free_power_power.png";
    public override string CustomBigIconPath => "res://images/powers/free_power_power.png";


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
            card.Type != CardType.Power)
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
            cardPlay.Card.Type != CardType.Power)
            return Task.CompletedTask;

        if (!cardPlay.IsFirstInSeries || cardPlay.IsAutoPlay)
            return Task.CompletedTask;

        var card = cardPlay.Card;
        bool savesEnergy = card.HasEnergyCostX ||
            card.EnergyCost.GetWithModifiers(CostModifiers.Local) > 0;
        bool savesChant = card is HinamoriModCard momo &&
            momo.ChantCostBeforeSingleUseFree > 0;
        if (!savesEnergy && !savesChant)
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
