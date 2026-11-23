
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Character;
using HinamoriMod.HinamoriModCode.Commands;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 赤火砲
/// </summary>
public class RedFrameCannonMomo : HinamoriModCard
{
    public override int ChantCost => 2;

    // カードのタグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Burn", 2m),
        new DynamicVar("Ember", 5m)
    ];

    public RedFrameCannonMomo()
        : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay)) return;

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // ダメージ
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // DamageCmd may trigger the default attack animation; switch to the kido cast afterward.
        HinamoriCombatAnimation.Play("cast");

        // 火傷2を付与
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

        // 火種5を獲得
        await EmberCmd.GainEmber(
            DynamicVars["Ember"].BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
