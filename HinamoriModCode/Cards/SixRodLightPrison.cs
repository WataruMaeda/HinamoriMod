
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「六杖光牢」
/// 2EN＋詠唱1
/// 敵1体に脱力2、12ブロック
/// 次に使用するパワーカードのEN・詠唱コスト0
/// 強化後：16ブロック
/// </summary>
public class SixRodLightPrisonMomo : HinamoriModCard
{
    public override int ChantCost => 1;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new DynamicVar("Weak", 2m)
    ];

    public SixRodLightPrisonMomo()
        : base(
            2,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // 敵に脱力2
        global::HinamoriMod.HinamoriModCode.Character.HinamoriCombatAnimation.Play("cast");

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars["Weak"].BaseValue,
            Owner.Creature,
            this);

        // ブロック12（強化後16）
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        // 次のパワーカードを無料化
        await PowerCmd.Apply<FreeChantPowerCardPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
