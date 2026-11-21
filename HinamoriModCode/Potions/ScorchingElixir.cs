using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using HinamoriMod.HinamoriModCode.Character;
using HinamoriMod.HinamoriModCode.Commands;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Potions;

/// <summary>灼熱の霊薬。画像・ビルド・ゲーム内動作は要検証。</summary>
[Pool(typeof(HinamoriModPotionPool))]
public class ScorchingElixir : CustomPotionModel
{
    public const int PrimaryAmount = 10;
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyEnemy;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null) return;
        await PowerCmd.Apply<BurnPower>(choiceContext, target, PrimaryAmount, Owner.Creature, null);
    }
}
