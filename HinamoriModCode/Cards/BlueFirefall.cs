
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
/// 破道「蒼火墜」：敵全体を攻撃し、火種を獲得する。
/// 通常：1EN＋詠唱1、全体8ダメージ、火種10。強化：全体10ダメージ、火種12。
/// </summary>
public class BlueFirefallMomo : HinamoriModCard
{
    // 追加の詠唱コスト。SpendChant で共通の軽減・消費を行う。
    public override int ChantCost => 1;

    // カードのタグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    // DamageVar は全体攻撃の基礎値、ForgeVar は獲得する火種の数。
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
        // 詠唱が足りない場合は、攻撃も火種獲得も実行しない。
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // TargetingAllOpponents で敵全体を対象にする。攻撃完了後に火種を付与する。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

        // 既存の EmberCmd に獲得量を渡し、雛森本人の火種を増やす。
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
