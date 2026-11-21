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

/// <summary>
/// 受け継ぐ想い。全キャラクターのパワーカードから毎ターン生成する。
/// キャラクター専用プールに雛森MODのプールを加えて抽選する。
/// TODO: スターコストの一時無料化。
/// </summary>
public class InheritedResolvePower : HinamoriModPower
{
    protected virtual bool UpgradeGeneratedCards => false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || !Owner.IsAlive || Amount <= 0)
            return;

        // 本体の全キャラクターと、現在のMODキャラクターのプールを対象にする。
        // 同一プール・同一カードは重複抽選しない。
        var candidates = ModelDb.AllCharacterCardPools
            .Append(player.Character.CardPool)
            .Distinct()
            .SelectMany(pool => pool.GetUnlockedCards(
                player.UnlockState,
                player.RunState.CardMultiplayerConstraint))
            .Where(card => card.Type == CardType.Power)
            .DistinctBy(card => card.Id)
            .ToList();

        if (candidates.Count == 0)
            return;

        for (int i = 0; i < Amount; i++)
        {
            var generated = CardFactory.GetDistinctForCombat(
                player, candidates, 1, player.RunState.Rng.CombatCardGeneration)
                .FirstOrDefault();

            if (generated == null)
                continue;

            if (UpgradeGeneratedCards && generated.IsUpgradable)
                CardCmd.Upgrade(generated);

            // 掴趾追雀と同じ、生成したターンのみの無料化。
            generated.SetToFreeThisTurn();
            if (generated is HinamoriModCard momoCard)
                momoCard.SetChantFreeThisTurn();

            // 廃棄・エセリアルは付与しない。
            await CardPileCmd.AddGeneratedCardToCombat(
                generated, PileType.Hand, player);
        }
    }
}
