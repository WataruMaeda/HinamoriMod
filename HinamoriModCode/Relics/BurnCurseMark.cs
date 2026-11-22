using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>火傷の呪印：火傷中の敵から受ける攻撃ダメージを追加で20%軽減。</summary>
public class BurnCurseMark : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override decimal ModifyDamageMultiplicative(
        Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        return target == Owner.Creature && props.IsPoweredAttack() &&
               dealer?.GetPower<BurnPower>() is not null
            ? 0.8m : 1m;
    }
}
