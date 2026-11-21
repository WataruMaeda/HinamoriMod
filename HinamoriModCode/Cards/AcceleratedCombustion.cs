using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 燃焼促進：対象の火傷スタックを2倍にする、廃棄付きアンコモンスキル。
/// 強化後は効果と廃棄を維持したまま、エナジーコストを2から1に下げる。
/// </summary>
public class AcceleratedCombustionMomo : HinamoriModCard
{
    // 使用後は廃棄する。強化後も廃棄キーワードは残す。
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    public AcceleratedCombustionMomo()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 対象の敵を必須とする。火傷のない敵に対しては追加処理をしない。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        if (!cardPlay.Target.IsAlive)
            return;

        BurnPower? burn = cardPlay.Target.GetPower<BurnPower>();
        if (burn == null || burn.Amount <= 0)
            return;

        // 既存スタックと同じ数だけ火傷を追加すると、結果として2倍になる。
        // BurnPowerの通常の付与経路を使い、スタックの直接書き換えは避ける。
        await PowerCmd.Apply<BurnPower>(
            choiceContext,
            cardPlay.Target,
            burn.Amount,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 強化時はエナジーのみ2→1に変更。倍率と廃棄は変わらない。
        EnergyCost.UpgradeBy(-1);
    }
}
