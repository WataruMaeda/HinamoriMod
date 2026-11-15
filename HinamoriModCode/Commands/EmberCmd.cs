
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

        if (amount <= 0m)
            return;

        // 火種を更新。既存カードも自動同期される。
        EmberState.AddAmount(player, amount);

        // 廃棄されていない飛梅を探す。
        bool hasTobiume = player.PlayerCombatState
            .AllCards
            .OfType<TobiumeMomo>()
            .Any(card =>
                !card.IsDupe &&
                card.Pile?.Type != PileType.Exhaust);

        if (!hasTobiume)
        {
            TobiumeMomo tobiume =
                player.Creature.CombatState
                .CreateCard<TobiumeMomo>(player);

            // 手札に追加する前に数値を確定する
            tobiume.SyncEmber();

            await CardPileCmd.AddGeneratedCardToCombat(
                tobiume,
                PileType.Hand,
                player);
        }
    }
}
