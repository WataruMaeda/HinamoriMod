
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
    // 追加で詠唱3を消費する。強化しても詠唱コストは変わらない。
    public override int ChantCost => 3;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

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
        // 詠唱不足なら、廃棄札の選択やカード回収は行わない。
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // 廃棄札をスナップショット化し、選択画面に渡す候補を固定する。
        List<CardModel> cards =
            PileType.Exhaust.GetPile(Owner)
                .Cards
                .ToList();

        if (cards.Count == 0)
            return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        // 選択をスキップさせず、回収するカードを1枚決める。
        CardModel selected =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                cards,
                Owner,
                canSkip: false);

        if (selected == null)
            return;

        // 回収後に再び自動廃棄されないよう、廃棄キーワードを取り除く。
        selected.RemoveKeyword(CardKeyword.Exhaust);

        // 手札に戻した後、ターン終了時に捨てられないよう保留を付ける。
        selected.AddKeyword(CardKeyword.Retain);

        // キーワードを変更してから、廃棄札のカードを手札へ移動する。
        await CardPileCmd.Add(selected, PileType.Hand);
    }


    protected override void OnUpgrade()
    {
        // 強化でエナジーコストを1→0に下げる。
        EnergyCost.UpgradeBy(-1);
    }
}
