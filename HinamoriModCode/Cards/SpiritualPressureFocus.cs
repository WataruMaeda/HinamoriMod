
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 霊圧集中
/// 通常：1EN、詠唱＋1、1ドロー
/// 強化：1EN、詠唱＋1、2ドロー
/// </summary>
public class SpiritualPressureFocusMomo : HinamoriModCard
{
    // 詠唱獲得量とドロー枚数
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Chant", 1m),
        new CardsVar(1)
    ];

    // 自分を対象にした1ENのスキルとして登録する。
    public SpiritualPressureFocusMomo()
        : base(
            1,
            CardType.Skill,
            CardRarity.Common,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 先に詠唱パワーを付与する。Owner.Creature は効果を受ける雛森本人。
        // DynamicVars を使うことでカード表示の値と実際の獲得量を一致させる。
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);

        // 詠唱獲得後にドローする。強化後は CardsVar が2になる。
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        // ドロー枚数：1 → 2
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
