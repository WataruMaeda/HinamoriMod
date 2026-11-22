
using System.Linq;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace HinamoriMod.HinamoriModCode.Commands;

/// <summary>
/// 火種の獲得を一元管理するコマンド。
/// 火種を増やすだけでなく、戦闘中に飛梅が存在しない場合の生成も担当する。
/// </summary>
public static class EmberCmd
{
    /// <summary>
    /// 指定プレイヤーの火種を増やす。戦闘終了中や獲得量0以下では何もしない。
    /// </summary>
    public static async Task GainEmber(
        decimal amount,
        Player player)
    {
        // 戦闘終了中の状態変更やカード生成を防ぐ。
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        // マイナス値や0は火種獲得として扱わない。
        if (amount <= 0m)
            return;

        // 火種を更新。既存カードも自動同期される。
        EmberState.AddAmount(player, amount);

        // 炎の循環：火種の獲得量にかかわらず、獲得処理1回につき1回発動する。
        // 通常版と強化版を別パワーとして保持し、混在時にも対象を区別する。
        var normalCycle = player.Creature.GetPower<FlameCyclePower>();
        var allCycle = player.Creature.GetPower<FlameCycleAllPower>();
        if ((normalCycle?.Amount ?? 0) > 0 || (allCycle?.Amount ?? 0) > 0)
        {
            var enemies = player.Creature.CombatState
                .GetOpponentsOf(player.Creature)
                .Where(enemy => enemy.IsAlive)
                .ToList();

            if (enemies.Count > 0)
            {
                if (normalCycle != null && normalCycle.Amount > 0)
                {
                    // Use the game's synchronized combat targeting RNG.
                    var target = player.RunState.Rng.CombatTargets
                        .NextItem(enemies)!;
                    await PowerCmd.Apply<BurnPower>(
                        new ThrowingPlayerChoiceContext(),
                        target,
                        normalCycle.Amount,
                        player.Creature,
                        null);
                }

                if (allCycle != null && allCycle.Amount > 0)
                {
                    foreach (var enemy in enemies)
                    {
                        if (!enemy.IsAlive)
                            continue;

                        await PowerCmd.Apply<BurnPower>(
                            new ThrowingPlayerChoiceContext(),
                            enemy,
                            allCycle.Amount,
                            player.Creature,
                            null);
                    }
                }
            }
        }

        // 戦闘中のカードから、複製でなく廃棄札にもない飛梅を探す。
        // 存在すれば追加生成せず、飛梅の重複を防ぐ。
        bool hasTobiume = player.PlayerCombatState
            .AllCards
            .OfType<TobiumeMomo>()
            .Any(card =>
                !card.IsDupe &&
                card.Pile?.Type != PileType.Exhaust);

        // 飛梅が存在しない場合だけ、新しい飛梅を手札に生成する。
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
