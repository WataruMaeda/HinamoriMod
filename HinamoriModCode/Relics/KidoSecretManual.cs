using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>鬼道の秘伝書：各ターン最初の破道・縛道の詠唱コストを0にする。</summary>
public class KidoSecretManual : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    private bool _usedThisTurn;

    // TODO: ターン開始時にResetForTurnを呼ぶ。
    // TODO: HinamoriModCard.EffectiveChantCostとSpendChantを連動させる。
    // TODO: 最初の破道・縛道が実際に使用された時だけMarkKidoPlayedを呼ぶ。
    // TODO: リプレイや詠唱0の鬼道での消費判定を検証する。
    public bool IsDiscountAvailable => !_usedThisTurn;
    public void ResetForTurn() => _usedThisTurn = false;
    public void MarkKidoPlayed() => _usedThisTurn = true;
}
