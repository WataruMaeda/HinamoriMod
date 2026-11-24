
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 破道「闐嵐」
/// 敵全体にダメージを与え、カードを2枚引く。
/// </summary>
public class RagingStormMomo : HinamoriModCard
{
    // 詠唱コスト
    public override int ChantCost => 2;

    // 破道・鬼道
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move),
        new CardsVar(2)
    ];

    public RagingStormMomo()
        : base(
            2,
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

        // 敵全体に15ダメージ（強化後19）
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

        // カードを2枚引く
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        // ダメージ 15 → 19
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
