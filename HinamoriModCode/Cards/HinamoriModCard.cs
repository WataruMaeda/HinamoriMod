
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HinamoriMod.HinamoriModCode.Character;
using HinamoriMod.HinamoriModCode.Extensions;
using HinamoriMod.HinamoriModCode.Powers;
using HinamoriMod.HinamoriModCode.Relics;
using System.Linq;
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

    /// <summary>
    /// 破道の極意による自動使用中だけ、通常の破道の詠唱を免除する。
    /// 黒棺は独自の全詠唱消費処理を維持する。
    /// </summary>
    private bool IsPinnacleAutoPlay =>
        IsMutable &&
        CombatState != null &&
        HasHinamoriTag(HinamoriCardTag.Hado) &&
        this is not BlackCoffinMomo &&
        Owner.Creature.GetPower<PinnacleOfHadoPower>()?.IsAutoPlaying(this) == true;

    /// <summary>
    /// アタックポーション・パワーポーション等の SetToFreeThisTurn による一時的な無料化。
    /// 通常の0ENパワーとは区別するため、一時スターコスト0も確認する。
    /// </summary>
    private bool IsTemporarilyFreePotionCard =>
        (Type == CardType.Power || Type == CardType.Attack) &&
        IsMutable &&
        CombatState != null &&
        TemporaryStarCost?.Cost == 0 &&
        EnergyCost.GetWithModifiers(CostModifiers.Local) == 0;

    public int EffectiveChantCost
    {
        get
        {
            if (ChantCost < 0)
                return -1;

            // アタックポーション・パワーポーション等による一時的な無料化
            if (IsTemporarilyFreePotionCard)
                return 0;

            // 破道の極意による自動使用
            if (IsPinnacleAutoPlay)
                return 0;

            // 掴趾追雀による無料化
            if (FreeChantThisTurn)
                return 0;

            // 鬼道の秘伝書：そのターン最初の破道・縛道は詠唱0。
            if (HasHinamoriTag(HinamoriCardTag.Kido) &&
                IsMutable && CombatState != null &&
                Owner.Relics.OfType<KidoSecretManual>().Any(r => r.IsDiscountAvailable))
                return 0;

            // 鬼道の秘伝書の無料化は、初回の実使用時だけ消費する。
        if (HasHinamoriTag(HinamoriCardTag.Kido) &&
            Owner.Relics.OfType<KidoSecretManual>().FirstOrDefault(r => r.IsDiscountAvailable)
                is { } secretManual)
        {
            secretManual.MarkKidoPlayed();
            return true;
        }

        // 鬼道の結晶：このターン中、鬼道の詠唱コスト0
            if (HasHinamoriTag(HinamoriCardTag.Kido) &&
                IsMutable && CombatState != null &&
                Owner.Creature.GetPower<PotionFreeKidoChantPower>() != null)
                return 0;

            // 詠唱破棄による鬼道の詠唱無料化
            if (HasHinamoriTag(HinamoriCardTag.Kido) &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeKidoChantPower>() != null)
            {
                return 0;
            }

            // 崩輪によるスキル無料化
            if (Type == CardType.Skill &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeChantSkillPower>() != null)
            {
                return 0;
            }

            // 百歩欄干によるアタック無料化
            if (Type == CardType.Attack &&
                IsMutable &&
                CombatState != null &&
                Owner.Creature.GetPower<FreeChantAttackPower>() != null)
            {
                return 0;
            }

            // 六杖光牢によるパワーカードの詠唱無料化
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

            // 鬼道の鍛錬は破道・縛道の両方に適用する。
            // 既存の破道専用軽減とは加算し、詠唱コストは0未満にしない。
            if (HasHinamoriTag(HinamoriCardTag.Kido) &&
                IsMutable &&
                CombatState != null)
            {
                discount += Owner.Creature
                    .GetPower<KidoDisciplinePower>()?.Amount ?? 0;
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

        // 騒乱など、効果による自動プレイでは詠唱を消費しない
        if (cardPlay.IsAutoPlay)
            return true;

        // アタックポーション・パワーポーション等による一時的な無料化
        if (IsTemporarilyFreePotionCard)
            return true;

        // 破道の極意で自動使用された破道は詠唱を消費しない
        if (IsPinnacleAutoPlay)
            return true;

        // 掴趾追雀で生成したカード
        if (FreeChantThisTurn)
            return true;

        // 鬼道の結晶：このターン中、鬼道の詠唱コスト0
        if (HasHinamoriTag(HinamoriCardTag.Kido) &&
            IsMutable && CombatState != null &&
            Owner.Creature.GetPower<PotionFreeKidoChantPower>() != null)
            return true;

        // 詠唱破棄による無料化
        if (HasHinamoriTag(HinamoriCardTag.Kido) &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeKidoChantPower>() != null)
        {
            return true;
        }

        // 崩輪による無料化
        if (Type == CardType.Skill &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeChantSkillPower>() != null)
        {
            return true;
        }

        // 百歩欄干によるアタック詠唱無料化
        if (Type == CardType.Attack &&
            IsMutable &&
            CombatState != null &&
            Owner.Creature.GetPower<FreeChantAttackPower>() != null)
        {
            return true;
        }

        // 六杖光牢によるパワーカードの詠唱無料化
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

        // 鬼道の護符：実際に消費した詠唱だけを累積する。
        if (cost > 0)
        {
            foreach (var talisman in Owner.Relics.OfType<KidoTalisman>())
            {
                int triggers = talisman.CountBlockTriggers(cost);
                for (int i = 0; i < triggers; i++)
                    await CreatureCmd.GainBlock(Owner.Creature,
                        new MegaCrit.Sts2.Core.Localization.DynamicVars.BlockVar(10m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered),
                        cardPlay);
            }
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
