using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 鬼道の探究：毎ターン開始時、火種があれば1消費して筋力・敏捷性を得る。
/// 重複した場合、各スタックが火種を1ずつ消費する。
/// </summary>
public class KidoResearchPower : HinamoriModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || !Owner.IsAlive || Amount <= 0)
            return;

        // 1スタックにつき火種1を消費し、筋力・敏捷性を1ずつ得る。
        for (int i = 0; i < Amount; i++)
        {
            if (EmberState.GetAmount(player) < 1m)
                break;

            EmberState.AddAmount(player, -1m);

            var context = new ThrowingPlayerChoiceContext();
            await PowerCmd.Apply<StrengthPower>(context, Owner, 1m, Owner, null);
            await PowerCmd.Apply<DexterityPower>(context, Owner, 1m, Owner, null);
        }
    }
}
