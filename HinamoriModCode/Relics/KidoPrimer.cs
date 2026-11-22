using System;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.CardSelection;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>鬼道の手引き：初期ドロー後、雛森専用カード3択から1枚を手札に加えて1枚捨てる。</summary>
public class KidoPrimer : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public const int CandidateCount = 3;
    public const int DiscardCount = 1;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || player.PlayerCombatState?.TurnNumber != 1)
            return;

        var unlocked = player.Character.CardPool.GetUnlockedCards(
            player.UnlockState, player.RunState.CardMultiplayerConstraint);
        var pool = unlocked
            .Where(card => card is HinamoriModCard)
            .Where(card => card.Rarity != CardRarity.Basic)
            .OrderBy(card => card.Id.Entry, StringComparer.Ordinal)
            .ToList();

        var options = CardFactory.GetDistinctForCombat(
            player, pool, CandidateCount, player.RunState.Rng.CombatCardGeneration).ToList();
        if (options.Count == 0)
            return;

        var selected = await CardSelectCmd.FromChooseACardScreen(
            choiceContext, options, player, canSkip: false);
        if (selected is null)
            return;

        await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, player);

        var prefs = new CardSelectorPrefs(
            CardSelectorPrefs.DiscardSelectionPrompt, selectCount: DiscardCount);
        var toDiscard = (await CardSelectCmd.FromHandForDiscard(
            choiceContext, player, prefs, filter: null, source: this)).ToList();
        if (toDiscard.Count > 0)
            await CardCmd.Discard(choiceContext, toDiscard);
    }
}
