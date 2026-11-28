using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 鬼道の探究：毎ターン開始時、火種があれば1消費して筋力・敏捷性を得る。
/// 重複した場合、各スタックが火種を1ずつ消費する。
/// </summary>
public class KidoResearchPower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/KidoResearchPower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/KidoResearchPower.png";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int UpgradedStacks { get; set; }

    public void AddUpgradedStack()
    {
        AssertMutable();
        UpgradedStacks++;
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || !Owner.IsAlive || Amount <= 0)
            return;

        // 強化済みのスタックは火種2をまとめて消費し、筋力・敏捷性を2ずつ得る。
        // 通常スタックは火種1を消費し、筋力・敏捷性を1ずつ得る。
        int upgraded = System.Math.Min(UpgradedStacks, Amount);
        int normal = Amount - upgraded;
        var context = new ThrowingPlayerChoiceContext();

        for (int i = 0; i < upgraded; i++)
        {
            if (EmberState.GetAmount(player) < 2m)
                continue;

            EmberState.AddAmount(player, -2m);
            await PowerCmd.Apply<StrengthPower>(context, Owner, 2m, Owner, null);
            await PowerCmd.Apply<DexterityPower>(context, Owner, 2m, Owner, null);
        }

        for (int i = 0; i < normal; i++)
        {
            if (EmberState.GetAmount(player) < 1m)
                break;

            EmberState.AddAmount(player, -1m);
            await PowerCmd.Apply<StrengthPower>(context, Owner, 1m, Owner, null);
            await PowerCmd.Apply<DexterityPower>(context, Owner, 1m, Owner, null);
        }
    }
}
