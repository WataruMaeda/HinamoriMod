using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace HinamoriMod.HinamoriModCode.Commands;

public static class EmberCmd
{
    public static async Task GainEmber(
        decimal amount,
        Player player)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        // 廃棄されていない飛梅を探す
        List<TobiumeMomo> cards = player.PlayerCombatState
            .AllCards
            .OfType<TobiumeMomo>()
            .Where(card =>
                !card.IsDupe &&
                card.Pile?.Type != PileType.Exhaust)
            .ToList();

        // 飛梅が存在しなければ手札に生成
        if (cards.Count == 0)
        {
            TobiumeMomo tobiume =
                player.Creature.CombatState
                    .CreateCard<TobiumeMomo>(player);

            await CardPileCmd.AddGeneratedCardToCombat(
                tobiume,
                PileType.Hand,
                player
            );
        }

        // すべての飛梅を強化
        foreach (TobiumeMomo card in player.PlayerCombatState
                     .AllCards
                     .OfType<TobiumeMomo>()
                     .Where(card => !card.IsDupe))
        {
            card.AdjustEmber(amount);
        }
    }
}