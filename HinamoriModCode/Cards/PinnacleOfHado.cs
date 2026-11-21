using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 破道の極意。使用時に破道自動使用パワーを付与する。
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

        await PowerCmd.Apply<PinnacleOfHadoPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
