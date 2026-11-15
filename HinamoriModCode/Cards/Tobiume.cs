
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 飛梅
/// </summary>
public class TobiumeMomo : HinamoriModCard
{
    private const decimal BaseDamage = 5m;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BaseDamage, ValueProp.Move),
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

    public override void AfterCreated()
    {
        base.AfterCreated();
        SyncEmber();
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        SyncEmber();
    }

    public void SyncEmber()
    {
        if (!IsMutable)
            return;

        if (Owner?.Creature.CombatState == null)
            return;

        SetEmber(EmberState.GetAmount(Owner));
    }


    /// <summary>
    /// プレイヤーが現在保持している火種。
    /// </summary>

    public decimal CurrentEmber
    {
        get
        {
            if (!IsMutable)
                return 0m;

            if (Owner?.Creature.CombatState == null)
                return 0m;

            return EmberState.GetAmount(Owner);
        }
    }


    /// <summary>
    /// プレイヤーの火種をカード表示に同期する。
    /// 火種そのものは変更しない。
    /// </summary>
    public void SetEmber(decimal amount)
    {
        AssertMutable();

        decimal ember = Math.Max(0m, amount);

        DynamicVars.Damage.BaseValue = BaseDamage + ember;
        DynamicVars["Burn"].BaseValue =
            1m + decimal.Floor(ember / 10m);
        DynamicVars["Ember"].BaseValue = ember;
    }

    /// <summary>
    /// 火種から火傷付与量を計算する。
    /// </summary>
    public int GetBurnAmount()
    {
        return 1 + (int)decimal.Floor(CurrentEmber / 10m);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // プレイヤーの最新の火種を反映
        SetEmber(CurrentEmber);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (cardPlay.Target.IsAlive)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                cardPlay.Target,
                GetBurnAmount(),
                Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
