
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Linq;
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

        var pile = PileType.Exhaust.GetPile(Owner);
        MegaCrit.Sts2.Core.Logging.Log.Info($"[ChainBinding] Exhaust: candidates={pile.Cards.Count}");

        if (pile.Cards.Count == 0)
            return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", 0f);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = (await CardSelectCmd.FromCombatPile(
            choiceContext, pile, Owner, prefs)).FirstOrDefault();

        MegaCrit.Sts2.Core.Logging.Log.Info($"[ChainBinding] selected={selected?.Id.Entry ?? "none"}");

        if (selected == null)
            return;

        // 回収後に再び自動廃棄されないよう、廃棄キーワードを取り除く。
        selected.RemoveKeyword(CardKeyword.Exhaust);

        // 手札に戻した後、ターン終了時に捨てられないよう保留を付ける。
        selected.AddKeyword(CardKeyword.Retain);

        // キーワードを変更してから、廃棄札のカードを手札へ移動する。
        await CardPileCmd.Add(selected, PileType.Hand);
        MegaCrit.Sts2.Core.Logging.Log.Info($"[ChainBinding] Exhaust: remaining={pile.Cards.Count}, hand={PileType.Hand.GetPile(Owner).Cards.Count}");
    }


    protected override void OnUpgrade()
    {
        // 強化でエナジーコストを1→0に下げる。
        EnergyCost.UpgradeBy(-1);
    }
}
