
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 詠唱準備
/// 通常：0EN、詠唱＋2、廃棄
/// 強化：0EN、詠唱＋2、廃棄なし
/// </summary>
public class ChantPreparationMomo : HinamoriModCard
{
    // 通常版は使用後に廃棄する。強化時にはこのキーワードを取り除く。
    // 通常版は廃棄
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    // 詠唱獲得量
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Chant", 2)];

    public ChantPreparationMomo()
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
        // 対象と発生源はどちらも使用者自身。詠唱量はカードの動的変数に従う。
        await PowerCmd.Apply<ChantPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Chant"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 強化では詠唱量を増やさず、廃棄キーワードだけを取り除く。
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
