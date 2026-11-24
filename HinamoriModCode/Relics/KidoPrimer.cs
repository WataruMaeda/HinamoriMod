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

/// <summary>鬼道の手引き：初期ドロー後、雛森専用カード3択から1枚を手札に加えて1枚廃棄する。生成カードはこのターン無料。</summary>
public class KidoPrimer : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public const int CandidateCount = 3;
    public const int ExhaustCount = 1;

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

        selected.SetToFreeThisTurn();
        if (selected is HinamoriModCard hinamoriCard)
            hinamoriCard.SetChantFreeThisTurn();

        await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, player);
        if (!PileType.Hand.GetPile(player).Cards.Any())
            return;

        var prefs = new CardSelectorPrefs(
            CardSelectorPrefs.ExhaustSelectionPrompt, selectCount: DiscardCount);
        var toExhaust = (await CardSelectCmd.FromHand(
            choiceContext, player, prefs, filter: null, source: this)).ToList();
        foreach (var card in toExhaust)
            await CardCmd.Exhaust(choiceContext, card);
    }
}
