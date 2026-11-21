using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>残火の欠片：火傷付与量+1。PowerCmd.Applyの共通入口への接続が残タスク。</summary>
public class LastEmberFragment : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    // TODO: BurnPower付与処理の共通経路でこのレリックの所持を確認して+1する。
    // TODO: 自身による追加付与が再帰発動しないよう、付与量を一度だけ補正する。
    public static decimal AdjustBurnAmount(decimal originalAmount) => originalAmount > 0m ? originalAmount + 1m : originalAmount;
}
