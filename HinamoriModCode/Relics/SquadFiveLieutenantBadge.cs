using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>五番隊副官章：各Actのエンシェント追加レリックを管理する。</summary>
public class SquadFiveLieutenantBadge : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int AncientBonusCount { get; set; }
}
