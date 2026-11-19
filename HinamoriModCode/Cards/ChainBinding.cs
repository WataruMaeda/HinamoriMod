
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「鎖条鎖縛」
/// 1EN＋詠唱3
/// 廃棄札1枚を手札に戻す。
/// 回収したカードの廃棄を除去し、保留を付与する。
/// 強化後：0EN＋詠唱3
/// </summary>
public class ChainBindingMomo : HinamoriModCard
{
    public override int ChantCost => 3;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Kido;

    public ChainBindingMomo()
        : base(
            1,
            CardType.Skill,
            CardRarity.Rare,
            TargetType.Self)
    {
    }


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // 廃棄札のカードを取得
        List<CardModel> cards =
            PileType.Exhaust.GetPile(Owner)
                .Cards
                .ToList();

        if (cards.Count == 0)
            return;

        // 廃棄札から1枚選択
        CardModel selected =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                cards,
                Owner,
                canSkip: false);

        if (selected == null)
            return;

        // 廃棄キーワードを除去
        selected.RemoveKeyword(CardKeyword.Exhaust);

        // 保留キーワードを付与
        selected.AddKeyword(CardKeyword.Retain);

        // 廃棄札から手札に戻す
        await CardPileCmd.Add(selected, PileType.Hand);
    }


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
