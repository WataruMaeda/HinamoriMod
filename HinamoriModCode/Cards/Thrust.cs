
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
/// 衝
/// 0コスト6ダメージ
/// </summary>
public class ThrustMomo : HinamoriModCard
{
    // 破道かつ鬼道として扱い、鬼道系のコスト軽減や使用回数判定に利用する。
    // カードのタグ
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    // 基礎ダメージ6。表示と実際の攻撃処理で同じ変数を使用する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public ThrustMomo()
        : base(
            0,
            CardType.Attack,
            CardRarity.Common,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 単体攻撃なので対象未指定の場合は処理を中断する。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // カード由来の攻撃として指定した敵にダメージを与える。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 強化後のダメージは6→9。
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
