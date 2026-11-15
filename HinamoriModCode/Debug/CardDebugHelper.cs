using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Debug;

internal static class CardDebugHelper
{
    public static IReadOnlyList<Type> GetCards() =>
        typeof(HinamoriModCard).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(HinamoriModCard).IsAssignableFrom(t))
            .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
            .OrderBy(t => t.Name)
            .ToArray();

    public static async Task AddCard(Player player, Type cardType, bool upgraded)
    {
        if (!typeof(HinamoriModCard).IsAssignableFrom(cardType))
            throw new ArgumentException("Not a Hinamori card", nameof(cardType));

        var state = player.Creature.CombatState;
        if (state == null || !CombatManager.Instance.IsInProgress)
            return;

        // ICombatState.CreateCard<T>(Player) is confirmed by CardPileCmd.cs.
        // Resolve the generic method dynamically so the selected card type can vary.
        var factory = state.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "CreateCard" && m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 &&
                m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(Player));
        if (factory == null)
            throw new MissingMethodException(state.GetType().FullName, "CreateCard<T>(Player)");

        var card = (CardModel?)factory.MakeGenericMethod(cardType).Invoke(state, new object[] { player });
        if (card == null)
            return;

        if (upgraded && !card.IsUpgraded)
        {
            card.UpgradeInternal();
            card.FinalizeUpgradeInternal();
        }

        // îÚî~ÇÃèÍçáÅAéËéDÇ…í«â¡Ç∑ÇÈëOÇ…åªç›ÇÃâŒéÌÇîΩâf
        if (card is TobiumeMomo tobiume)
        {
            tobiume.SyncEmber();
        }

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
    }
}
