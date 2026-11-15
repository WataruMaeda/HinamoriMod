
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 破道「双蓮蒼火墜」
/// 敵全体に2回ダメージを与え、火傷5を付与する。
/// </summary>
public class TwinLotusBlueFirefallMomo : HinamoriModCard
{
    public override int ChantCost => 3;

    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Hado | HinamoriCardTag.Kido;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new IntVar("Hits", 2),
        new IntVar("Burn", 5)
    ];

    public TwinLotusBlueFirefallMomo()
        : base(
            2,
            CardType.Attack,
            CardRarity.Rare,
            TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 詠唱を消費
        if (!await SpendChant(choiceContext, cardPlay))
            return;

        // 敵全体に2ヒット攻撃
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars["Hits"].IntValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

        // 生存している敵全体に火傷5を付与
        var enemies = CombatState
            .GetOpponentsOf(Owner.Creature)
            .Where(enemy => enemy.IsAlive)
            .ToList();

        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                enemy,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 10ダメージ×2 → 12ダメージ×2
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
