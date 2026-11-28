using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 鬼道の反復：手札の鬼道を1枚選ぶ。
/// 掴趾追雀と同じ BaseReplayCount で戦闘中持続するリプレイを付与する。
/// </summary>
public class KidoRecitationMomo : HinamoriModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Replay", 1m)
    ];

    public KidoRecitationMomo()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        var selected = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(
                new LocString("card_selection", "HINAMORIMOD_TO_REPLAY"), 1),
            context: choiceContext,
            player: Owner,
            filter: card => card is HinamoriModCard momo
                && momo.HasHinamoriTag(HinamoriCardTag.Kido),
            source: this)).FirstOrDefault();

        if (selected != null)
            selected.BaseReplayCount += DynamicVars["Replay"].IntValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Replay"].UpgradeValueBy(1m);
    }
}
