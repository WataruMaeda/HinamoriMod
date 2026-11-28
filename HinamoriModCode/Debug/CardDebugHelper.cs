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
    public enum DebugCardCategory { Hinamori, Status, Curse }

    public static IReadOnlyList<Type> GetCards(DebugCardCategory category)
    {
        if (category == DebugCardCategory.Hinamori)
            return GetCards();

        // 本体カードの型を列挙。デバッグ用途なのでゲームが認識する
        // CardModelの派生クラスから生成可能なものを選別する。
        var wanted = category == DebugCardCategory.Status ? CardType.Status : CardType.Curse;
        return typeof(CardModel).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(CardModel).IsAssignableFrom(t))
            .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
            .Where(t =>
            {
                try
                {
                    var model = CreateModel(t);
                    return model != null && model.Type == wanted;
                }
                catch { return false; }
            })
            .OrderBy(t => t.Name).ToArray();
    }

    private static CardModel? CreateModel(Type type)
    {
        var method = typeof(ModelDb).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "Card" && m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 0);
        return method?.MakeGenericMethod(type).Invoke(null, null) as CardModel;
    }

    public static IReadOnlyList<Type> GetCards() =>
        typeof(HinamoriModCard).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(HinamoriModCard).IsAssignableFrom(t))
            .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
            .OrderBy(t => t.Name)
            .ToArray();

    public static async Task AddCard(Player player, Type cardType, bool upgraded, PileType destination = PileType.Hand)
    {
        if (!typeof(CardModel).IsAssignableFrom(cardType) || cardType.IsAbstract)
            throw new ArgumentException("Not a card model", nameof(cardType));

        if (destination != PileType.Hand && destination != PileType.Draw &&
            destination != PileType.Discard && destination != PileType.Exhaust)
            throw new ArgumentOutOfRangeException(nameof(destination));

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

        // 飛梅の場合、手札に追加する前に現在の火種を反映
        if (card is TobiumeMomo tobiume)
        {
            tobiume.SyncEmber();
        }

        await CardPileCmd.AddGeneratedCardToCombat(card, destination, player);
    }
}
