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

/// <summary>完全詠唱の秘薬。画像・ビルド・ゲーム内動作は要検証。</summary>
[Pool(typeof(HinamoriModPotionPool))]
public class PerfectIncantationElixir : CustomPotionModel
{
    public const int PrimaryAmount = 10;
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<ChantPower>(choiceContext, Owner.Creature, PrimaryAmount, Owner.Creature, null);
        await PowerCmd.Apply<FullIncantationPower>(choiceContext, Owner.Creature, 2m, Owner.Creature, null);
    }
}
