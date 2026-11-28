using System;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 炎の吸収：対象の火傷をすべて消費し、火傷3につきHPを1回復するレアスキル。
/// 通常2EN、強化後1EN。使用後は廃棄せず、火傷3未満なら回復量は0。
/// </summary>
public class FlameAbsorptionMomo : HinamoriModCard
{
    public FlameAbsorptionMomo()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        // 敵1体の火傷を消費するカードのため、対象が必要。
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        if (!cardPlay.Target.IsAlive)
            return;

        BurnPower? burn = cardPlay.Target.GetPower<BurnPower>();
        if (burn == null || burn.Amount <= 0)
            return;

        // 火傷はすべて消費する。端数は切り捨て、火傷3未満は回復しない。
        int consumedBurn = burn.Amount;
        int healAmount = consumedBurn / 3;

        // 消費処理は既存のパワー除去コマンドを使い、火傷の状態を残さない。
        await PowerCmd.Remove(burn);

        if (healAmount <= 0)
            return;

        // 回復処理はゲーム標準のコマンドに委譲する。
        // 最大HPの上限はゲーム側の回復処理に従う。
        await CreatureCmd.Heal(Owner.Creature, healAmount);
    }

    protected override void OnUpgrade()
    {
        // 強化は2EN→1ENのみ。回復効率と廃棄なしは変更しない。
        EnergyCost.UpgradeBy(-1);
    }
}
