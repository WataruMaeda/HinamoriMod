using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 鬼道の極意。強化後は縛道も自動使用する。
/// ドロー時の自動使用処理はPinnacleOfHadoPowerで行う。
/// </summary>
public class PinnacleOfHadoMomo : HinamoriModCard
{
    public override int ChantCost => 3;

    public PinnacleOfHadoMomo()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);


        var existing = Owner.Creature.GetPower<PinnacleOfHadoPower>();
        if (existing == null)
        {
            await PowerCmd.Apply<PinnacleOfHadoPower>(
                choiceContext, Owner.Creature, IsUpgraded ? 2m : 1m, Owner.Creature, this);
        }
        else if (IsUpgraded && existing.Amount < 2)
        {
            await PowerCmd.ModifyAmount(
                choiceContext, existing, 2 - existing.Amount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 強化後は縛道も自動使用する。
    }
}
