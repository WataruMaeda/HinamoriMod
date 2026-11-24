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
        var candidates = pile.Cards.ToList();
        MegaCrit.Sts2.Core.Logging.Log.Info($"[LieutenantsIngenuity] {pileType}: candidates={candidates.Count}");

        if (candidates.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = pileType == PileType.Draw
            ? (await CardSelectCmd.FromSimpleGrid(
                choiceContext, candidates, Owner, prefs)).FirstOrDefault()
            : (await CardSelectCmd.FromCombatPile(
                choiceContext, pile, Owner, prefs)).FirstOrDefault();

        MegaCrit.Sts2.Core.Logging.Log.Info($"[LieutenantsIngenuity] {pileType}: selected={selected?.Id.Entry ?? "none"}");

        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Hand);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[LieutenantsIngenuity] {pileType}: remaining={pile.Cards.Count}, hand={PileType.Hand.GetPile(Owner).Cards.Count}");
        }
    }

    protected override void OnUpgrade()
    {
        // 強化後は捨て札からも回収する。
    }
}
