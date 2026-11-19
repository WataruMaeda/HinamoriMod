using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 藍染隊長の仇
/// 3EN：敵1体にダメージ、自分にブロック、対象に火傷を与える。
/// 使用するたびに、ダメージとブロックは (1 + 強化回数)、火傷は1ずつ成長する。
/// 成長値はデッキ側にも反映し、戦闘・階層をまたいで保持する。
/// </summary>
public class AvengeCaptainAizenMomo : HinamoriModCard
{
    private const int BaseAmount = 1;

    private int _currentAmount = BaseAmount;
    private int _increasedAmount;
    private int _currentBlockAmount = BaseAmount;
    private int _currentBurnAmount = BaseAmount;

    // ダメージの現在値（旧セーブとの互換性のため既存名を維持）
    [SavedProperty]
    public int CurrentAmount
    {
        get => _currentAmount;
        set
        {
            AssertMutable();
            _currentAmount = value;
            UpdateVars();
        }
    }

    // 使用によるダメージの累積成長値（既存名を維持）
    [SavedProperty]
    public int IncreasedAmount
    {
        get => _increasedAmount;
        set
        {
            AssertMutable();
            _increasedAmount = value;
        }
    }

    // ブロックの現在値
    [SavedProperty]
    public int CurrentBlockAmount
    {
        get => _currentBlockAmount;
        set
        {
            AssertMutable();
            _currentBlockAmount = value;
            UpdateVars();
        }
    }

    // 火傷の現在値
    [SavedProperty]
    public int CurrentBurnAmount
    {
        get => _currentBurnAmount;
        set
        {
            AssertMutable();
            _currentBurnAmount = value;
            UpdateVars();
        }
    }

    public override int MaxUpgradeLevel => 999999999;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentAmount, ValueProp.Move),
        new BlockVar(CurrentBlockAmount, ValueProp.Move),
        new IntVar("Burn", CurrentBurnAmount),
        new IntVar("Growth", CurrentUpgradeLevel + 1)
    ];

    public AvengeCaptainAizenMomo()
        : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 使用前の数値で効果を発動
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue,
            ValueProp.Move,
            cardPlay,
            false);

        if (cardPlay.Target.IsAlive)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this);
        }

        // 強化回数に応じて、使用後に成長する
        IncreaseFromPlay();

        // 戦闘中のコピーだけでなく、デッキの原本にも成長を記録
        if (DeckVersion is AvengeCaptainAizenMomo deckCard &&
            !ReferenceEquals(deckCard, this))
        {
            deckCard.IncreaseFromPlay();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Growth"].BaseValue =
            CurrentUpgradeLevel + 1;
    }

    private void IncreaseFromPlay()
    {
        int growth = DynamicVars["Growth"].IntValue;

        // ダメージ・ブロックの成長量
        // 火傷の成長量は常に1
        IncreasedAmount += growth;
        CurrentAmount += growth;
        CurrentBlockAmount += growth;
        CurrentBurnAmount += 1;
    }

    private void UpdateVars()
    {
        DynamicVars.Damage.BaseValue = CurrentAmount;
        DynamicVars.Block.BaseValue = CurrentBlockAmount;
        DynamicVars["Burn"].BaseValue = CurrentBurnAmount;
    }
}
