using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 鬼道の真髄：コモンのパワー。通常1EN、強化後0EN。
/// 鬼道を戦闘中に3回使うたびに1枚引く。カウントはターンを跨ぐ。
/// </summary>
public class EssenceOfKidoMomo : HinamoriModCard
{
    public EssenceOfKidoMomo()
        : base(1, CardType.Power, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 複数枚設置した場合、パワーのAmountを重ねてドロー量を増やす。
        await PowerCmd.Apply<EssenceOfKidoPower>(
            choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 強化後は設置コストを1→0にする。
        EnergyCost.UpgradeBy(-1);
    }
}
