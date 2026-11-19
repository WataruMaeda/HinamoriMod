
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 霊圧解放
/// 通常：2EN、手札の他のカード1枚をランダム廃棄、
///       詠唱+5
/// 強化：2EN、廃棄カードを選択可能、詠唱+5
/// </summary>
public class SpiritualPressureReleaseMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Chant", 5m)
    ];

    public SpiritualPressureReleaseMomo()
        : base(
            2,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // カード使用中の自身は通常手札から取り除かれている。
        // 念のため自身を候補から除外する。
        var candidates = PileType.Hand.GetPile(Owner).Cards
            .Where(card => !ReferenceEquals(card, this))
            .ToList();

        if (candidates.Count > 0)
        {
            CardModel? selectedCard;

            if (IsUpgraded)
            {
                // 標準の「不屈」と同じ手札選択処理。
                // プレイ中の自身は手札に存在しない想定。
                selectedCard = (await CardSelectCmd.FromHand(
                    prefs: new CardSelectorPrefs(
                        CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                    context: choiceContext,
                    player: Owner,
                    filter: null,
                    source: this)).FirstOrDefault();
            }
            else
            {
                selectedCard = Owner.RunState.Rng.CombatCardSelection
                    .NextItem(candidates);
            }

            if (selectedCard != null)
            {
                await CardCmd.Exhaust(choiceContext, selectedCard);
            }
        }

        // 詠唱+5
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 強化後は廃棄カードを選択可能にする
    }
}
