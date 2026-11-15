using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

public class TobiumeMomo : HinamoriModCard
{
    private decimal _currentDamage = 5m;
    private decimal _currentEmber = 0m;

    // 保留
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];

    // 基本ダメージ5
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Burn", 1m),
        new DynamicVar("Ember", 0m)
    ];

    public TobiumeMomo()
        : base(
            2,
            CardType.Attack,
            CardRarity.Token,
            TargetType.AnyEnemy)
    {
    }

    // 現在の火種
    public decimal CurrentEmber => _currentEmber;

    // 火種を獲得
    public void AdjustEmber(decimal amount)
    {
        AssertMutable();

        if (amount <= 0m)
            return;

        decimal next = decimal.Max(0m, _currentEmber + amount);

        _currentEmber = next;
        _currentDamage =  5m + next;

        // ダメージを更新
        DynamicVars.Damage.BaseValue = _currentDamage;

        // 火傷の付与量を更新
        DynamicVars["Burn"].BaseValue = GetBurnAmount();

        // 現在の火種を更新
        DynamicVars["Ember"].BaseValue = _currentEmber;
    }

    // 火種から火傷の付与量を計算
    public int GetBurnAmount()
    {
        return 1 + (int)decimal.Floor(_currentEmber / 10m);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // ダメージ
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 火傷を付与
        await PowerCmd.Apply<BurnPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars["Burn"].IntValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}