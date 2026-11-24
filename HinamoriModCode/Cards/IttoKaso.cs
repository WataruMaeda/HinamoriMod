using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 破道の九十六「一刀火葬」：赤火砲の古代カード版。
/// 0EN＋詠唱2。敵全体に20（強化25）ダメージ、火傷5、火種10、自己HP5減少。
/// </summary>
public class IttoKasoMomo : HinamoriModCard
{
    public override int ChantCost => 2;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Move),
        new IntVar("Burn", 5),
        new ForgeVar(10),
        new IntVar("SelfHpLoss", 5)
    ];

    public IttoKasoMomo()
        : base(0, CardType.Attack, CardRarity.Basic, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

        var enemies = CombatState.GetOpponentsOf(Owner.Creature)
            .Where(enemy => enemy.IsAlive)
            .ToList();

        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                enemy,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this);
        }

        await EmberCmd.GainEmber(DynamicVars.Forge.IntValue, Owner);

        // ブロックを無視してHPを直接失う（非攻撃ダメージ）。
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars["SelfHpLoss"].IntValue,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
