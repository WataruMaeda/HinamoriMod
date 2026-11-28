
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 破道の六十三「飛竜撃賊震天雷炮」
///
/// 通常：10 + この戦闘でプレイした破道の回数 × 3
/// 強化：10 + この戦闘でプレイした破道の回数 × 4
///
/// 自身の今回の使用はダメージ計算に含めない。
/// 使用完了後は次回以降の破道使用回数に含まれる。
/// </summary>
public class FlyingDragonStrikingHeavenShakingThunderCannonMomo
    : HinamoriModCard
{
    private const string CalculatedHitsKey = "CalculatedHits";

    public override int ChantCost => 2;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    /// <summary>
    /// この戦闘で使用が完了した破道の回数。
    /// </summary>
    private int GetHadoPlayCount()
    {
        if (CombatState == null)
            return 0;

        return CombatManager.Instance.History.CardPlaysFinished
            .Count(entry =>
                entry.CardPlay.Player == Owner &&
                entry.CardPlay.Card is HinamoriModCard card &&
                card.HasHinamoriTag(HinamoriCardTag.Hado));
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 破道1回あたりの追加ダメージ
        new DamageVar(3m, ValueProp.Move),

        // 基本ダメージ
        new IntVar("NormalDamage", 10m),

        // フィニッシャーと同じ計算用変数
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),

        // 破道使用回数
        new CalculatedHitCountVar(CalculatedHitsKey)
            .WithMultiplier((CardModel card, Creature? _) =>
            {
                var flyingDragon =
                    (FlyingDragonStrikingHeavenShakingThunderCannonMomo)card;

                return flyingDragon.GetHadoPlayCount();
            })
    ];

    public FlyingDragonStrikingHeavenShakingThunderCannonMomo()
        : base(
            1,
            CardType.Attack,
            CardRarity.Rare,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 使用完了済みの破道だけを数える
        int hadoCount = GetHadoPlayCount();

        // 基本ダメージ + 破道回数 × 追加ダメージ
        decimal damage =
            DynamicVars["NormalDamage"].IntValue
            + DynamicVars.Damage.BaseValue * hadoCount;

        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 破道1回あたりの追加ダメージを3→4に変更
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
