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
/// 使用するたびに、ダメージ・ブロックがそれぞれ (1 + 強化回数) 成長する。火傷はダメージの半分（切り捨て）。
/// 成長値はデッキ側にも反映し、戦闘・階層をまたいで保持する。
/// </summary>
public class AvengeCaptainAizenMomo : HinamoriModCard
{
    private const int BaseAmount = 10;

    private int _currentAmount = BaseAmount;
    private int _increasedAmount;
    private int _currentBlockAmount = BaseAmount;

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

    // 旧セーブ互換のためプロパティ名を維持。火傷は常に現在の基礎ダメージの半分。
    // Derived value: never serialize independently of CurrentAmount.
    public int CurrentBurnAmount
    {
        get => CurrentAmount / 2;
        set
        {
            AssertMutable();
            // 旧セーブの火傷値は使用せず、ダメージから再計算する。
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
            DynamicVars.Block,
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

        // 成長量は従来どおり。火傷はダメージから自動計算する。
        IncreasedAmount += growth;
        CurrentAmount += growth;
        CurrentBlockAmount += growth;
    }

    private void UpdateVars()
    {
        DynamicVars.Damage.BaseValue = CurrentAmount;
        DynamicVars.Block.BaseValue = CurrentBlockAmount;
        DynamicVars["Burn"].BaseValue = CurrentBurnAmount;
    }
}
