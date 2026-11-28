using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 炎の守り：自分に8ブロックを付与し、火種を5獲得するコモンスキル。
/// 強化後もブロックは8のまま、獲得する火種が5から8に増える。
/// </summary>
public class FlameGuardMomo : HinamoriModCard
{
    // ブロック獲得カードであることをゲーム本体に通知する。
    public override bool GainsBlock => true;

    // BlockVarはブロック値、ForgeVarは火種獲得量を表す。
    // どちらもカード説明と実際の処理で共通して参照する。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new ForgeVar(5)
    ];

    public FlameGuardMomo()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        // 先に使用者へブロックを付与する。BlockVarを使ってブロック補正を反映する。
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        // 次に火種を獲得する。飛梅の生成判定も既存の共通コマンドへ委譲する。
        await EmberCmd.GainEmber(DynamicVars.Forge.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        // 強化ではブロック値を変えず、火種獲得量だけ5から8へ増やす。
        DynamicVars.Forge.UpgradeValueBy(3);
    }
}
