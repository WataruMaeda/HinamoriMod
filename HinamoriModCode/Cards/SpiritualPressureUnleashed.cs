using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>霊圧の解放：エセリアル。毎ターン開始時にエナジーを1得る。</summary>
public class SpiritualPressureUnleashedMomo : HinamoriModCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SpiritualPressureUnleashedPower>(1m)
    ];

    public SpiritualPressureUnleashedMomo()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        await PowerCmd.Apply<SpiritualPressureUnleashedPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
