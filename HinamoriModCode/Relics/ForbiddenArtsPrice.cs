using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>禁術の代償：毎ターン1ENを得て、本体のランダムな状態異常を手札に加える。</summary>
public class ForbiddenArtsPrice : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public const int ExtraEnergyPerTurn = 1;
    public const int StatusCardsPerTurn = 1;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner || !Owner.Creature.IsAlive)
            return;

        await PlayerCmd.GainEnergy(ExtraEnergyPerTurn, player);
        var combat = player.Creature.CombatState;
        if (combat is null)
            return;

        var statuses = ModelDb.CardPool<StatusCardPool>().AllCards
            .Where(card => card.GetType().Assembly == typeof(StatusCardPool).Assembly)
            .OrderBy(card => card.Id.Entry, System.StringComparer.Ordinal)
            .ToList();

        var canonical = player.RunState.Rng.CombatCardGeneration.NextItem(statuses);
        if (canonical is null)
            return;

        var generated = combat.CreateCard(canonical, player);
        await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, player);
    }
}
