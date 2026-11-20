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

/// <summary>飛梅の霊髄。画像・ビルド・ゲーム内動作は要検証。</summary>
[Pool(typeof(HinamoriModPotionPool))]
public class TobiumeEssence : CustomPotionModel
{
    public const int PrimaryAmount = 30;
    public const int BurnAmount = 5;
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await EmberCmd.GainEmber(PrimaryAmount, Owner);
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).Where(x => x.IsAlive).ToList())
            await PowerCmd.Apply<BurnPower>(choiceContext, enemy, BurnAmount, Owner.Creature, null);
    }
}
