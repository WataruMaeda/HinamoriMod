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
/// 飛梅の炎撃：2ENで敵1体に12ダメージを与え、火傷3を付与するコモンアタック。
/// 強化後はダメージ12を維持し、付与する火傷を3から5に増やす。
/// </summary>
public class TobiumesFlameStrikeMomo : HinamoriModCard
{
    // 攻撃ダメージと火傷スタック数を動的変数として定義し、説明文にも反映する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new IntVar("Burn", 3)
    ];

    public TobiumesFlameStrikeMomo()
        : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 敵1体への攻撃なので、対象が未指定の場合は処理を行わない。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // カード由来のダメージ補正を適用して、指定した敵を攻撃する。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 攻撃で敵が倒れた場合は火傷を付与しない。既存の火傷パワーを使用する。
        if (cardPlay.Target.IsAlive)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        // 強化後は火傷3→5。ダメージは12のまま。
        DynamicVars["Burn"].UpgradeValueBy(2m);
    }
}
