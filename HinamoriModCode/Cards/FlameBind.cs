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
/// 炎の拘束：敵1体に火傷2を付与し、自分に5ブロックを得るコモンスキル。
/// 強化後はブロックを変えず、付与する火傷だけを2から3に増やす。
/// </summary>
public class FlameBindMomo : HinamoriModCard
{
    // ブロック獲得カードとして扱い、ブロック関連のゲーム処理と連携する。
    public override bool GainsBlock => true;

    // 表示と実際の処理で同じブロック量・火傷スタック数を参照する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new IntVar("Burn", 2)
    ];

    public FlameBindMomo()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 火傷を付与する敵1体を必須とする。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // カード一覧の記載順に、まず対象の敵へ火傷を付与する。
        if (cardPlay.Target.IsAlive)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this);
        }

        // 次に自分へ5ブロックを得る。ブロック補正は既存の共通処理に委譲する。
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        // 強化は火傷2→3のみ。ブロック5とエナジー1は変更しない。
        DynamicVars["Burn"].UpgradeValueBy(1m);
    }
}
