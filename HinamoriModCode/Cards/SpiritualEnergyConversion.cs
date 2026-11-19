using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 霊力変換
/// 通常：0EN＋詠唱3、2EN獲得
/// 強化：0EN＋詠唱3、3EN獲得
/// 詠唱コスト軽減を適用する。
/// </summary>
public class SpiritualEnergyConversionMomo : HinamoriModCard
{
    public override int ChantCost => 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2)
    ];

    public SpiritualEnergyConversionMomo()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
