
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 廃炎
/// </summary>
public class AbolishingFlamesMomo : HinamoriModCard
{
    // 詠唱コスト
    public override int ChantCost => 3;

    // カードのタグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move),
        new IntVar("Burn", 5)
    ];

    public AbolishingFlamesMomo()
        : base(
            1,
            CardType.Attack,
            CardRarity.Uncommon,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱を消費
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 敵1体にダメージ
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 対象が生存していれば火傷5を付与
        if (cardPlay.Target.IsAlive)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        // ダメージ15 → 18
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
