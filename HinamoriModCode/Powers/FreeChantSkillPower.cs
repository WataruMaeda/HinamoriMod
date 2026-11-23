
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 次に使用するスキルのEN・詠唱コストを0にする。
/// ターンをまたいで持続する。
/// </summary>
public class FreeChantSkillPower : HinamoriModPower
{
    // 本編の「急襲」が付与する FreeSkillPower と同じアイコンを使用する。
    // 効果（EN・詠唱コストの両方を0にする）はMOD独自のまま維持。
    public override string CustomPackedIconPath =>
        "res://images/atlases/power_atlas.sprites/freeskillpower.tres";

    public override string CustomBigIconPath =>
        "res://images/powers/freeskillpower.png";

    // パワーが存在する状態で使用開始したカードを記録
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
            card.Type != CardType.Skill)
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
            cardPlay.Card.Type != CardType.Skill)
            return Task.CompletedTask;

        // 無料化の対象だったカードだけ記録する
        _eligiblePlays.Add(cardPlay);

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // このパワーの付与前に使用開始したカードは対象外
        if (!_eligiblePlays.Remove(cardPlay))
            return;

        // 対象スキルの使用完了後に消費
        await PowerCmd.Decrement(this);
    }
}
