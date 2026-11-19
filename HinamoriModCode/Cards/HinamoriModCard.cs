
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
    /// ’Íæä’Ç‚É‚æ‚éˆê“I‚È‰r¥–³—¿‰»
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

            // ’Íæä’Ç‚É‚æ‚é–³—¿‰»
            if (FreeChantThisTurn)
                return 0;

            // ‰r¥”jŠü‚É‚æ‚é‹S“¹‚Ì‰r¥–³—¿‰»
            if (HasHinamoriTag(HinamoriCardTag.Kido) &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeKidoChantPower>() != null)
            {
                return 0;
            }

            // •ö—Ö‚É‚æ‚éƒXƒLƒ‹–³—¿‰»
            if (Type == CardType.Skill &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeChantSkillPower>() != null)
            {
                return 0;
            }

            // •S•à—“Š±‚É‚æ‚éƒAƒ^ƒbƒN–³—¿‰»
            if (Type == CardType.Attack &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeChantAttackPower>() != null)
            {
                return 0;
            }

            // ˜ZñŒõ˜S‚É‚æ‚éƒpƒ[ƒJ[ƒh‚Ì‰r¥–³—¿‰»
            if (Type == CardType.Power &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeChantPowerCardPower>() != null)
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
    /// ‰r¥ƒRƒXƒg‚ğÁ”ï‚·‚éB
    /// ƒŠƒvƒŒƒC‚ÍÄÁ”ï‚µ‚È‚¢B
    /// </summary>
    protected async Task<bool> SpendChant(
        PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (ChantCost < 0)
            return true;

        // ƒŠƒvƒŒƒC‚Å‚Í‰r¥‚ğÄÁ”ï‚µ‚È‚¢
        if (!cardPlay.IsFirstInSeries)
            return true;

        // ’Íæä’Ç‚Å¶¬‚µ‚½ƒJ[ƒh
        if (FreeChantThisTurn)
            return true;

        // ‰r¥”jŠü‚É‚æ‚é–³—¿‰»
        if (HasHinamoriTag(HinamoriCardTag.Kido) &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeKidoChantPower>() != null)
        {
            return true;
        }

        // •ö—Ö‚É‚æ‚é–³—¿‰»
        if (Type == CardType.Skill &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeChantSkillPower>() != null)
        {
            return true;
        }

        // •S•à—“Š±‚É‚æ‚éƒAƒ^ƒbƒN‰r¥–³—¿‰»
        if (Type == CardType.Attack &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeChantAttackPower>() != null)
        {
            return true;
        }

        // ˜ZñŒõ˜S‚É‚æ‚éƒpƒ[ƒJ[ƒh‚Ì‰r¥–³—¿‰»
        if (Type == CardType.Power &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeChantPowerCardPower>() != null)
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

        // ”j“¹‚Ì‰r¥ŒyŒ¸‚ğÁ”ï
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
