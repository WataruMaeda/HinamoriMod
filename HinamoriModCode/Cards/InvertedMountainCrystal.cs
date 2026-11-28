
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「倒山晶」
/// 通常：0EN＋詠唱4、25ブロック
/// 強化：30ブロック
/// 現在のブロックを次のターンまで維持する
/// </summary>
public class InvertedMountainCrystalMomo : HinamoriModCard
{
    public override int ChantCost => 4;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(25m, ValueProp.Move),
        new DynamicVar("Blur", 1m)
    ];

    public InvertedMountainCrystalMomo()
        : base(
            0,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱4を消費
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // ブロック25獲得（強化後30）
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        // ブロックを次のターンまで維持
        await PowerCmd.Apply<BlurPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Blur"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // ブロック25 → 30
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}
