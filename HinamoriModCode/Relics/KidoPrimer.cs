using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>鬼道の手引き：戦闘開始時に3枚から1枚選び、手札1枚を捨てる。</summary>
public class KidoPrimer : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public const int CandidateCount = 3;
    public const int DiscardCount = 1;
    // TODO: 戦闘開始時の初期ドロー後に候補を3枚抽選する。
    // TODO: PlayerChoiceContextを使用して1枚を選択し手札に生成する。
    // TODO: 選択したカードを含む手札から1枚選んで捨てる。
    // TODO: レアリティ制限・候補重複除外・マルチプレイ同期を確認する。
}
