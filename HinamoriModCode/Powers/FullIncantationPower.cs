
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Powers;

public class FullIncantationPower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/icons/FullIncantation_icon.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/icons/FullIncantation_icon.png";

    private readonly HashSet<CardPlay> _eligiblePlays = new();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => true;

    private bool IsTargetCard(CardModel? card)
    {
        return card is HinamoriModCard hinamoriCard
            && card.Owner.Creature == Owner
            && (hinamoriCard.HasHinamoriTag(HinamoriCardTag.Hado)
                || hinamoriCard.HasHinamoriTag(HinamoriCardTag.Bakudo));
    }

    private decimal Multiplier => Amount >= 2 ? 2m : 1.5m;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return IsTargetCard(cardSource) ? Multiplier : 1m;
    }

    public override decimal ModifyBlockMultiplicative(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return IsTargetCard(cardSource) ? Multiplier : 1m;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (IsTargetCard(cardPlay.Card))
            _eligiblePlays.Add(cardPlay);

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!_eligiblePlays.Remove(cardPlay))
            return;

        await PowerCmd.Remove(this);
    }
}
