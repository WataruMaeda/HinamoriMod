
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
/// 破道「黒棺」
/// 現在の詠唱をすべて消費し、
/// 消費した詠唱×6（強化後×8）のダメージ。
/// リプレイ時は初回の消費詠唱量を再利用する。
/// </summary>
public class BlackCoffinMomo : HinamoriModCard
{
    // 同一カード使用中のリプレイで参照する値
    private int _lastChantSpent;

    // 固定の詠唱コストは持たない
    public override int ChantCost => -1;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public BlackCoffinMomo()
        : base(
            0,
            CardType.Attack,
            CardRarity.Rare,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 手動の初回使用時のみ詠唱を取得・消費する。
        // 騒乱などの自動プレイでは詠唱を消費せず、ダメージも0とする。
        if (cardPlay.IsFirstInSeries && cardPlay.IsAutoPlay)
            _lastChantSpent = 0;
        if (cardPlay.IsFirstInSeries && !cardPlay.IsAutoPlay)
        {
            var chant =
                Owner.Creature.GetPower<ChantPower>();

            _lastChantSpent = Math.Max(
                0,
                chant?.Amount ?? 0);

            if (_lastChantSpent > 0 && chant != null)
            {
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    chant,
                    -_lastChantSpent,
                    Owner.Creature,
                    this);
            }
        }

        // リプレイ時も初回の消費詠唱量を使用
        decimal damage =
            DynamicVars.Damage.BaseValue * _lastChantSpent;

        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
