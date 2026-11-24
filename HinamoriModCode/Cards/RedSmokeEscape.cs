
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「赤煙遁」
/// 1EN、8ブロック、1ドロー
/// 強化後：11ブロック、1ドロー
/// </summary>
public class RedSmokeEscapeMomo : HinamoriModCard
{
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    // ブロックを得るカードとしてゲーム本体へ通知する。
    public override bool GainsBlock => true;

    // BlockVar はブロック値、CardsVar はドロー枚数。強化でブロックのみ増える。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new CardsVar(1)
    ];

    public RedSmokeEscapeMomo()
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
        // まず使用者にブロックを付与する。BlockVar から各種補正を反映する。
        global::HinamoriMod.HinamoriModCode.Character.HinamoriCombatAnimation.Play("cast");

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        // ブロック獲得後にカードを1枚引く。強化後もドロー枚数は同じ。
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        // ブロック8 → 11
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
