
using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 瞬歩
/// 1コスト 5ブロック
/// </summary>
public class ShunpoMomo : HinamoriModCard
{
    // ブロックを獲得するカード
    public override bool GainsBlock => true;

    // 防御カードのタグ
    protected override HashSet<CardTag> CanonicalTags =>
        new() { CardTag.Defend };

    // 基本ブロック：5
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, ValueProp.Move)];

    // コスト1、スキル、ベーシック、自分対象
    public ShunpoMomo() : base(
        1,
        CardType.Skill,
        CardRarity.Basic,
        TargetType.Self)
    {
    }

    // カード使用時：ブロックを獲得
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        HinamoriCombatAnimation.Play("block");

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);
    }

    // 強化後：5 → 8
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
