
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 蒼火墜
/// </summary>
public class BlueFirefallMomo : HinamoriModCard
{
    public override int ChantCost => 1;

    // カードのタグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new ForgeVar(10)
    ];

    public BlueFirefallMomo()
        : base(
            1,
            CardType.Attack,
            CardRarity.Uncommon,
            TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱を消費
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // 敵全体に8ダメージ（強化後10）
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

        // 火種を獲得
        await EmberCmd.GainEmber(
            DynamicVars.Forge.IntValue,
            Owner
        );
    }

    protected override void OnUpgrade()
    {
        // ダメージ 8 → 10
        DynamicVars.Damage.UpgradeValueBy(2m);

        // 火種 10 → 12
        DynamicVars.Forge.UpgradeValueBy(2);
    }
}
