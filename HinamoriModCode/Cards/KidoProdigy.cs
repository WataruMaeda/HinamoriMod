
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 鬼道の才
/// 通常：1EN、詠唱+1、手札1枚を戦闘中強化
/// 強化：1EN、詠唱+1、手札すべてを戦闘中強化
/// </summary>
public class KidoProdigyMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Chant", 1m)
    ];

    public KidoProdigyMomo()
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
        await CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        // 詠唱+1
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);

        // 強化版：手札の強化可能なカードをすべて強化する。
        if (IsUpgraded)
        {
            // 列挙中に手札の状態が変わっても安全なようにスナップショット化する。
            foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards
                         .Where(c => c.IsUpgradable).ToList())
            {
                CardCmd.Upgrade(card);
            }

            return;
        }

        // 通常版：手札から強化可能なカードを1枚選択して強化する。
        CardModel selectedCard = await CardSelectCmd.FromHandForUpgrade(
            choiceContext, Owner, this);

        if (selectedCard != null)
        {
            CardCmd.Upgrade(selectedCard);
        }
    }

    protected override void OnUpgrade()
    {
        // 強化後は対象を1枚から手札全体へ変更
    }
}
