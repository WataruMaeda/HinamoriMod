
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HinamoriMod.HinamoriModCode.Character;
using HinamoriMod.HinamoriModCode.Extensions;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Cards;

[Pool(typeof(HinamoriModCardPool))]
public abstract class HinamoriModCard(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType target)
    : CustomCardModel(cost, type, rarity, target)
{
    public virtual int ChantCost => -1;

    public virtual bool IsHado => false;

    public virtual HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.None;

    /// <summary>
    /// 掴趾追雀による一時的な詠唱無料化
    /// </summary>
    public bool FreeChantThisTurn { get; private set; }

    public void SetChantFreeThisTurn()
    {
        AssertMutable();
        FreeChantThisTurn = true;
    }

    public void ClearChantFreeThisTurn()
    {
        AssertMutable();
        FreeChantThisTurn = false;
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (IsMutable && FreeChantThisTurn)
        {
            foreach (var creature in participants)
            {
                if (creature == Owner.Creature)
                {
                    FreeChantThisTurn = false;
                    break;
                }
            }
        }

        return base.AfterSideTurnEnd(choiceContext, side, participants);
    }

    public bool HasHinamoriTag(HinamoriCardTag tag) =>
        (HinamoriTags & tag) == tag;

    public int EffectiveChantCost
    {
        get
        {
            if (ChantCost < 0)
                return -1;

            // 掴趾追雀による無料化
            if (FreeChantThisTurn)
                return 0;

            // 崩輪によるスキル無料化
            if (Type == CardType.Skill &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeChantSkillPower>() != null)
            {
                return 0;
            }

            int discount = 0;

            if (HasHinamoriTag(HinamoriCardTag.Hado) &&
                IsMutable &&
                CombatState != null)
            {
                discount =
                    Owner.Creature
                        .GetPower<HadoChantDiscountPower>()
                        ?.Amount ?? 0;
            }

            return Math.Max(0, ChantCost - discount);
        }
    }

    protected override bool IsPlayable =>
        base.IsPlayable &&
        (!IsMutable ||
         CombatState == null ||
         EffectiveChantCost < 0 ||
         (Owner.Creature.GetPower<ChantPower>()?.Amount ?? 0)
            >= EffectiveChantCost);

    /// <summary>
    /// 詠唱コストを消費する。
    /// リプレイ時は再消費しない。
    /// </summary>
    protected async Task<bool> SpendChant(
        PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (ChantCost < 0)
            return true;

        // リプレイでは詠唱を再消費しない
        if (!cardPlay.IsFirstInSeries)
            return true;

        // 掴趾追雀で生成したカード
        if (FreeChantThisTurn)
            return true;

        // 崩輪による無料化
        if (Type == CardType.Skill &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeChantSkillPower>() != null)
        {
            return true;
        }

        int cost = EffectiveChantCost;

        var chant =
            Owner.Creature.GetPower<ChantPower>();

        if ((chant?.Amount ?? 0) < cost)
            return false;

        if (cost > 0 && chant != null)
        {
            await PowerCmd.ModifyAmount(
                context,
                chant,
                -cost,
                Owner.Creature,
                this);
        }

        // 破道の詠唱軽減を消費
        if (HasHinamoriTag(HinamoriCardTag.Hado))
        {
            var discount =
                Owner.Creature
                    .GetPower<HadoChantDiscountPower>();

            if (discount != null)
                await PowerCmd.Remove(discount);
        }

        return true;
    }

    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .BigCardImagePath();

    public override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .CardImagePath();

    public override string BetaPortraitPath =>
        $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .CardImagePath();
}
