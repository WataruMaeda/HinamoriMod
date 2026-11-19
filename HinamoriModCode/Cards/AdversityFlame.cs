using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 逆境の炎
/// 通常：1EN。手札の状態異常をすべて廃棄し、廃棄した1枚につき火種を2得る。
/// 強化：1EN。状態異常に加えて呪いも廃棄対象にする。
/// </summary>
public class AdversityFlameMomo : HinamoriModCard
{
    // カード1枚を廃棄したときに得られる火種の数。
    // 今回の強化ではこの数値は変わらず、廃棄対象のみ増える。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Ember", 2m)
    ];

    public AdversityFlameMomo()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 手札にあるカードを確認する。
        // 廃棄中に手札の内容が変わるため、先に対象一覧を固定しておく。
        // このカード自身は処理対象に含めない。
        var targets = PileType.Hand.GetPile(Owner).Cards
            .Where(card =>
                !ReferenceEquals(card, this) &&
                (card.Type == CardType.Status ||
                 (IsUpgraded && card.Type == CardType.Curse)))
            .ToList();

        int exhaustedCount = 0;

        foreach (CardModel target in targets)
        {
            // 既存の「霊圧解放」と同じ本体コマンドでカードを廃棄する。
            await CardCmd.Exhaust(choiceContext, target);

            // 廃棄後に実際に廃棄札へ移動したカードだけを数える。
            // 廃棄できなかったカードには火種を付与しない。
            if (PileType.Exhaust.GetPile(Owner).Cards.Contains(target))
            {
                exhaustedCount++;
            }
        }

        // 対象が0枚、または実際に廃棄できたカードが0枚なら火種は得ない。
        // 火種はカードごとではなく合計値を1回で付与する。
        if (exhaustedCount > 0)
        {
            await EmberCmd.GainEmber(
                DynamicVars["Ember"].BaseValue * exhaustedCount,
                Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // IsUpgraded が true になると、OnPlay の判定に呪いが追加される。
        // エナジーコストとカード1枚あたりの火種獲得量は変わらない。
    }
}
