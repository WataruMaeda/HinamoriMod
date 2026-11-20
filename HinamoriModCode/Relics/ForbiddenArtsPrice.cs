using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>禁術の代償：毎ターンEN+1、ランダムな状態異常カード1枚を手札に加える。</summary>
public class ForbiddenArtsPrice : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public const int ExtraEnergyPerTurn = 1;
    public const int StatusCardsPerTurn = 1;
    // TODO: エナジーリセット後の追加EN付与を接続する。
    // TODO: 状態異常カードのプールからランダムに1枚を選び手札に生成する。
    // TODO: 状態異常の種類・虚無などの即時効果・手札上限・マルチプレイ同期を検証する。
}
