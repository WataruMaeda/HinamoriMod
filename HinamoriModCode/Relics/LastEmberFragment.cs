using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>残火の欠片：所持者が与えるすべての火傷付与量を1増加。</summary>
public class LastEmberFragment : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override decimal ModifyPowerAmountGivenAdditive(
        PowerModel power, Creature giver, decimal amount,
        Creature? target, CardModel? cardSource)
    {
        return power is BurnPower && giver == Owner.Creature && amount > 0m
            ? 1m : 0m;
    }
}
