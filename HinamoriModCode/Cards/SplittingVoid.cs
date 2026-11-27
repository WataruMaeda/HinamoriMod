
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「断空」
/// 通常：1EN＋詠唱2、バッファ1
/// 強化：1EN＋詠唱1、バッファ1
/// </summary>
public class SplittingVoidMomo : HinamoriModCard
{
    public override int ChantCost => IsUpgraded ? 1 : 2;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    public SplittingVoidMomo()
        : base(
            1,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        await PowerCmd.Apply<BufferPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 強化後の詠唱コストは
        // ChantCostで切り替える
    }
}
