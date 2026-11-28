using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 霊圧制御：アンコモン・1EN。
/// 鬼道を使うたび3ブロックを獲得。強化後は天賦。
/// </summary>
public class SpiritualPressureControlMomo : HinamoriModCard
{
    public SpiritualPressureControlMomo()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        // スタック量をブロック値として保持する。
        await PowerCmd.Apply<SpiritualPressureControlPower>(
            choiceContext, Owner.Creature, 3, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // ブロック量は変えず、天賦のみ追加する。
        AddKeyword(CardKeyword.Innate);
    }
}
