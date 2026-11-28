using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 破道「白雷」：敵1体にダメージを与え、その後カードを引く。
/// 通常：1EN、9ダメージ＋1ドロー。強化：10ダメージ＋2ドロー。
/// </summary>
public class WhiteLightningMomo : HinamoriModCard
{
    // カードのタグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    // DamageVar は攻撃ダメージ、CardsVar はドロー枚数をカード表示と同期する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new CardsVar(1)
    ];

    public WhiteLightningMomo()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 単体攻撃のため対象が必要。対象未指定なら不正な処理を続けない。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // ダメージ計算にはカード由来の補正を適用し、選択した敵へ攻撃する。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 攻撃の処理が終わってから、CardsVar で指定した枚数を引く。
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        // 強化でダメージ9→10、ドロー1→2。
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
