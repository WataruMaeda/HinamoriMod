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
/// 炎の爆発：敵1体に6ダメージを与え、火傷ダメージを即座に1回発動して火傷を1減らす。
/// 強化後は火傷ダメージを2回発動する。各回で現在の火傷数を参照する。
/// </summary>
public class FlameExplosionMomo : HinamoriModCard
{
    // 通常の攻撃ダメージと火傷の即時発動回数を表示・処理で共用する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new IntVar("Triggers", 1)
    ];

    public FlameExplosionMomo()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // まずカード由来の攻撃ダメージを与える。火傷の即時発動はその後に処理する。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        for (int i = 0; i < DynamicVars["Triggers"].IntValue; i++)
        {
            // 対象が死亡済み、または火傷が0になった時点で残りの発動は行わない。
            if (!cardPlay.Target.IsAlive)
                break;

            BurnPower? burn = cardPlay.Target.GetPower<BurnPower>();
            if (burn == null || burn.Amount <= 0)
                break;

            // BurnPowerのターン開始時と同じく、現在の火傷数を防御不能・非攻撃ダメージとして与える。
            // 通常の攻撃ダメージとは別処理なので、火傷による攻撃力低下の影響を受けない。
            await CreatureCmd.Damage(
                choiceContext,
                cardPlay.Target,
                burn.Amount,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                null);

            // ダメージで敵が倒れた場合、以降の火傷減少・発動は不要。
            if (!cardPlay.Target.IsAlive)
                break;

            // 即時発動1回につき火傷を1減らす。次回は減少後の火傷数を参照する。
            await PowerCmd.Decrement(burn);
        }
    }

    protected override void OnUpgrade()
    {
        // 強化では即時発動回数のみ1→2。通常攻撃6ダメージと1ENは変更しない。
        DynamicVars["Triggers"].UpgradeValueBy(1m);
    }
}
