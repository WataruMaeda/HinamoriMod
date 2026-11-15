
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 火傷
/// ・攻撃ダメージを25%減少
/// ・ターン開始時に火傷の数値と同じダメージ
/// ・ダメージ後に火傷を1減少
/// </summary>
public class BurnPower : HinamoriModPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 脱力と同じ攻撃ダメージ低下効果
    /// </summary>
    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        // 火傷を持つキャラクターの攻撃のみ対象
        if (dealer != Owner)
            return 1m;

        // 通常の攻撃ダメージのみ対象
        if (!props.IsPoweredAttack())
            return 1m;

        // 攻撃ダメージを25%減少
        return 0.75m;
    }

    /// <summary>
    /// ターン開始時の火傷ダメージ
    /// </summary>
    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        // 火傷を持つキャラクターのターン開始時のみ
        if (!participants.Contains(Owner))
            return;

        if (!Owner.IsAlive || Amount <= 0)
            return;

        // 現在の火傷数と同じダメージ
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Amount,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null
        );
    }

    /// <summary>
    /// ターン終了時の火傷減少処理
    /// </summary>
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        // 火傷を持つキャラクターのターン終了時のみ
        if (!participants.Contains(Owner))
            return;

        // 生存している場合は火傷を1減少
        if (Owner.IsAlive)
        {
            await PowerCmd.Decrement(this);
        }
    }
}
