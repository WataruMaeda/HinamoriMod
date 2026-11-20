using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>鬼道の研鑽。掴趾追雀のカードプール・生成処理を再利用する。</summary>
public class KidoStudyPower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || !Owner.IsAlive || Amount <= 0)
            return;

        var candidates = player.Character.CardPool
            .GetUnlockedCards(
                player.UnlockState,
                player.RunState.CardMultiplayerConstraint)
            .Where(card => card is HinamoriModCard)
            .ToList();

        if (candidates.Count == 0)
            return;

        for (int i = 0; i < Amount; i++)
        {
            var generated = CardFactory.GetDistinctForCombat(
                player,
                candidates,
                1,
                player.RunState.Rng.CombatCardGeneration)
                .FirstOrDefault();

            if (generated == null)
                continue;

            generated.AddKeyword(CardKeyword.Exhaust);
            generated.AddKeyword(CardKeyword.Ethereal);

            // TODO: 強化済みの鬼道の研鑽が生成した場合、カードを強化する。
            await CardPileCmd.AddGeneratedCardToCombat(
                generated, PileType.Hand, player);
        }
    }
}
