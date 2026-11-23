
using System.Collections.Generic;
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
    // 本編の無料化パワーと同じアイコンを使用する（EN・詠唱コストの処理は変更しない）。
    public override string CustomPackedIconPath =>
        "res://images/atlases/power_atlas.sprites/freepowerpower.tres";

    public override string CustomBigIconPath =>
        "res://images/powers/freepowerpower.png";

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
