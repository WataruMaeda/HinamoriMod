
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「掴趾追雀」
/// 2EN＋詠唱2
/// 雛森専用カード3枚から1枚を選び、手札に加える。
/// 生成カードはこのターンのみEN・詠唱コスト0。
/// 生成カードに戦闘中持続するリプレイ2を付与する。
/// 強化後：リプレイ3。
/// </summary>
public class TracingSparrowMomo : HinamoriModCard
{
    public override int ChantCost => 2;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Replay", 2m)
    ];

    public TracingSparrowMomo()
        : base(
            2,
            CardType.Skill,
            CardRarity.Rare,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱を消費（リプレイ時は再消費しない）
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // 雛森専用カードの候補を取得
        // 掴趾追雀自身も除外しない
        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card is HinamoriModCard)
            .ToList();

        if (candidates.Count == 0)
            return;

        // ランダムに異なるカードを最大3枚生成
        List<CardModel> cards =
            CardFactory.GetDistinctForCombat(
                Owner,
                candidates,
                3,
                Owner.RunState.Rng.CombatCardGeneration)
            .ToList();

        if (cards.Count == 0)
            return;

        global::HinamoriMod.HinamoriModCode.Character.HinamoriCombatAnimation.Play("cast");

        // 3枚から必ず1枚を選択（選択のスキップは不可）
        CardModel selected =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                cards,
                Owner,
                canSkip: false);

        if (selected == null)
            return;

        // このターンのみENコストを無料化（ターン終了時はゲーム本体が解除）
        selected.SetToFreeThisTurn();

        // 雛森独自の詠唱コストもこのターンのみ無料化
        // ※ターン終了時の解除処理はHinamoriModCard側で行う
        if (selected is HinamoriModCard momoCard)
        {
            momoCard.SetChantFreeThisTurn();
        }

        // 選択したカードに戦闘中持続するリプレイを付与
        // ※ExtraReplayThisTurnではなくBaseReplayCountを使用する
        selected.BaseReplayCount +=
            DynamicVars["Replay"].IntValue;

        // 選択したカードを手札に追加
        await CardPileCmd.AddGeneratedCardToCombat(
            selected,
            PileType.Hand,
            Owner);
    }

    protected override void OnUpgrade()
    {
        // リプレイ2 → 3
        DynamicVars["Replay"].UpgradeValueBy(1m);
    }
}
