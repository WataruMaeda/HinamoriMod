using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 副隊長の機転：山札から1枚選んで手札へ移動し、手札1枚を廃棄。
/// 強化後は捨て札からも1枚回収する。
/// </summary>
public class LieutenantsIngenuityMomo : HinamoriModCard
{
    public LieutenantsIngenuityMomo()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        await RetrieveCard(choiceContext, PileType.Draw);

        if (IsUpgraded)
            await RetrieveCard(choiceContext, PileType.Discard);

        if (!PileType.Hand.GetPile(Owner).Cards.Any(card => !ReferenceEquals(card, this)))
            return;

        var selected = (await CardSelectCmd.FromHand(
            context: choiceContext,
            player: Owner,
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
            filter: card => !ReferenceEquals(card, this),
            source: this)).FirstOrDefault();

        if (selected != null)
            await CardCmd.Exhaust(choiceContext, selected);
    }

    private async Task RetrieveCard(PlayerChoiceContext choiceContext, PileType pileType)
    {
        var pile = pileType.GetPile(Owner);
        if (!pile.Cards.Any())
            return;

        // 既存の戦闘パイルから同期された選択を行う。
        var selected = (await CardSelectCmd.FromCombatPile(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            context: choiceContext,
            pile: pile,
            player: Owner))
            .FirstOrDefault();

        if (selected != null)
            await CardPileCmd.Add(selected, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        // 強化後は捨て札からも回収する。
    }
}
