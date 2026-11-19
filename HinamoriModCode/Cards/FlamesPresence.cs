
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 炎の気配
/// 通常：0EN、火種+5、廃棄
/// 強化：0EN、火種+5、廃棄なし
/// </summary>
public class FlamesPresenceMomo : HinamoriModCard
{
    // 通常版は廃棄
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    // 火種獲得量
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Ember", 5m)
    ];

    public FlamesPresenceMomo()
        : base(
            0,
            CardType.Skill,
            CardRarity.Common,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 火種+5
        await EmberCmd.GainEmber(
            DynamicVars["Ember"].BaseValue,
            Owner);
    }

    protected override void OnUpgrade()
    {
        // 強化後は廃棄しない
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
