
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 詠唱破棄
/// 通常：2EN、次の破道・縛道の詠唱コスト0
/// 強化：1EN、効果は同じ
/// </summary>
public class IncantationAbandonmentMomo : HinamoriModCard
{
    public IncantationAbandonmentMomo()
        : base(
            2,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        await PowerCmd.Apply<FreeKidoChantPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
