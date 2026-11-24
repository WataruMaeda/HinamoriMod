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
    // エナジーは不要だが、詠唱を3消費する（詠唱無料化・軽減は共通処理に従う）。
    public override int ChantCost => 3;

    // 獲得するエナジー量。カード説明文の Energy 表示にも使用する。
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
        // 詠唱が不足する場合は効果を発動しない。リプレイでは二重消費しない。
        if (!await SpendChant(choiceContext, cardPlay))
            return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);


        // 詠唱を消費できたときだけエナジーを増やす。
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        // 強化で獲得エナジーを2→3に変更する。
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
