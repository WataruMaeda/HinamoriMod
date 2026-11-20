using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace HinamoriMod.HinamoriModCode.Relics;

/// <summary>
/// 鬼道の秘伝書：毎ターン最初の破道・縛道の詠唱コストを0にする。
/// 実際のコスト免除はカードのSpendChant側との接続が必要。
/// </summary>
public class KidoSecretManual : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
}
