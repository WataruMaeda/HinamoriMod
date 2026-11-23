using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Powers;

/// <summary>
/// 破道を引くとEN・詠唱消費なしで自動使用する。
/// 黒棺は例外として全詠唱を消費する。
/// </summary>
public class PinnacleOfHadoPower : HinamoriModPower
{
    // 雛森専用のパワーアイコン。
    public override string CustomPackedIconPath => "res://HinamoriMod/images/powers/PinnacleOfHadoPower.png";
    public override string CustomBigIconPath => "res://HinamoriMod/images/powers/big/PinnacleOfHadoPower.png";

    private sealed class Data
    {
        public readonly HashSet<CardModel> AutoPlayingCards = new();
        public int InfiniteAutoPlaysThisTurn;
    }

    private const int InfiniteAutoPlayCap = 9;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => true;

    protected override object InitInternalData() => new Data();

    public bool IsAutoPlaying(CardModel card) =>
        GetInternalData<Data>().AutoPlayingCards.Contains(card);

    public override async Task AfterCardDrawnEarly(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner ||
            card is not HinamoriModCard hinamoriCard ||
            !hinamoriCard.HasHinamoriTag(HinamoriCardTag.Hado))
            return;

        var data = GetInternalData<Data>();

        // 本体ヘルレイザーと同様、無限HPの敵だけがいる場合の連鎖を制限。
        if (Owner.CombatState.HittableEnemies.All(enemy => enemy.HpDisplay.IsInfinite()))
        {
            if (data.InfiniteAutoPlaysThisTurn >= InfiniteAutoPlayCap)
                return;

            data.InfiniteAutoPlaysThisTurn++;
        }
        else
        {
            data.InfiniteAutoPlaysThisTurn = 0;
        }

        data.AutoPlayingCards.Add(card);
        try
        {
            // 本体の自動使用処理がEN免除とランダム対象選択を担当。
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
        finally
        {
            data.AutoPlayingCards.Remove(card);
        }
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            GetInternalData<Data>().InfiniteAutoPlaysThisTurn = 0;

        return Task.CompletedTask;
    }
}
