using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 燃え盛る炎：プレイヤーの各ターン開始時、敵全体へ火傷を付与する。
/// 複数回適用すると付与量が加算される。
/// </summary>
public class BlazingTobiumePower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || !Owner.IsAlive || Amount <= 0)
            return;

        var enemies = Owner.CombatState.GetOpponentsOf(Owner)
            .Where(enemy => enemy.IsAlive)
            .ToList();

        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive)
                continue;

            await PowerCmd.Apply<BurnPower>(
                new ThrowingPlayerChoiceContext(),
                enemy,
                Amount,
                Owner,
                null);
        }
    }
}
