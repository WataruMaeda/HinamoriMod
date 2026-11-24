using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 飛梅の火花：敵全体に火傷2を付与するコモンスキル。
/// 強化後は付与する火傷を3に増やす。エナジーコストは1のまま。
/// </summary>
public class TobiumesSparkMomo : HinamoriModCard
{
    // 敵全体へ付与する火傷数を、説明文と処理で共通の変数として扱う。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Burn", 2)
    ];

    public TobiumesSparkMomo()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Attack", 0f);

        // 既存の双蓮蒼火墜と同じ方法で、生存している敵全体を取得する。
        // 付与中に敵の状態が変化しても列挙が崩れないようリストを固定する。
        var enemies = CombatState
            .GetOpponentsOf(Owner.Creature)
            .Where(enemy => enemy.IsAlive)
            .ToList();

        foreach (var enemy in enemies)
        {
            // 火傷は既存のBurnPowerで管理し、各敵へ2（強化後3）付与する。
            if (!enemy.IsAlive)
                continue;

            await PowerCmd.Apply<BurnPower>(
                choiceContext,
                enemy,
                DynamicVars["Burn"].IntValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        // 強化後は敵全体への火傷2→3のみ。コストは変わらない。
        DynamicVars["Burn"].UpgradeValueBy(1m);
    }
}
