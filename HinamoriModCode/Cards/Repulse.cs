
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 縛道「斥」
/// 1EN、5ブロック
/// このターン、ブロックで防いだ攻撃ダメージを攻撃者に反射する。
/// 強化後：8ブロック
/// </summary>
public class RepulseMomo : HinamoriModCard
{
    public override HinamoriCardTag HinamoriTags =>
        HinamoriCardTag.Bakudo | HinamoriCardTag.Kido;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move)
    ];

    public RepulseMomo()
        : base(
            1,
            CardType.Skill,
            CardRarity.Common,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // ブロックを獲得
        global::HinamoriMod.HinamoriModCode.Character.HinamoriCombatAnimation.Play("cast");

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        // 反射効果を付与
        await PowerCmd.Apply<ReflectPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // ブロック5 → 8
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
