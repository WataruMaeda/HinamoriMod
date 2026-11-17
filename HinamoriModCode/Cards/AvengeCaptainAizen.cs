
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
/// 使用するたびにダメージ・ブロック・火傷が1ずつ成長する。
/// 成長値は戦闘・階層をまたいで保持する。
/// </summary>
public class AvengeCaptainAizenMomo : HinamoriModCard
{
    private const int BaseAmount = 1;

    private int _currentAmount = BaseAmount;
    private int _increasedAmount;

    // 現在の効果量
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

    // 使用による累積成長値
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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentAmount, ValueProp.Move),
        new BlockVar(CurrentAmount, ValueProp.Move),
        new IntVar("Burn", CurrentAmount)
    ];

    public AvengeCaptainAizenMomo()
        : base(
            3,
            CardType.Attack,
            CardRarity.Uncommon,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 1. 敵1体にダメージ
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 2. 自分にブロック
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue,
            ValueProp.Move,
            cardPlay,
            false);

        // 3. 対象に火傷
        if (cardPlay.Target.IsAlive)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this);
        }

        // 4. 使用後に成長
        IncreaseFromPlay();

        // 5. デッキ側のカードにも反映
        (DeckVersion as AvengeCaptainAizenMomo)?
            .IncreaseFromPlay();
    }

    protected override void OnUpgrade()
    {
        // 3EN → 2EN
        EnergyCost.UpgradeBy(-1);
    }

    private void IncreaseFromPlay()
    {
        IncreasedAmount += 1;
        CurrentAmount = BaseAmount + IncreasedAmount;
    }

    private void UpdateVars()
    {
        DynamicVars.Damage.BaseValue = CurrentAmount;
        DynamicVars.Block.BaseValue = CurrentAmount;
        DynamicVars["Burn"].BaseValue = CurrentAmount;
    }
}
