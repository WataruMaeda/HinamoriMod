using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 精神統一：1ENで詠唱を2獲得する初期スキル。強化後は詠唱を3獲得する。
/// </summary>
public class MeditationMomo : HinamoriModCard
{
    // カード説明文と詠唱獲得処理で共通して使う数値。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Chant", 2)];

    public MeditationMomo()
        : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 自分自身に詠唱パワーを付与する。現在の獲得量はDynamicVarsから取得する。
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 強化時は詠唱の獲得量を2→3に変更する。
        DynamicVars["Chant"].UpgradeValueBy(1m);
    }
}