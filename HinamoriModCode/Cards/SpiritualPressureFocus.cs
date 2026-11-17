
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
        // 詠唱＋1
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);

        // カードを1枚引く（強化後は2枚）
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
