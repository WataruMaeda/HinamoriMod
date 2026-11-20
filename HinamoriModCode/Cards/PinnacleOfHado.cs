using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 破道の極意。カード使用とパワー付与までは実装済み。
/// ドロー時の自動使用は本体API確認後に実装する。
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
