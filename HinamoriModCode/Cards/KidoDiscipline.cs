using System.Collections.Generic;
using System.Threading.Tasks;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace HinamoriMod.HinamoriModCode.Cards;

/// <summary>
/// 詠唱破棄：2ENのアンコモンパワー。
/// 戦闘中、鬼道の詠唱コストを1減らす（最低0）。
/// 強化すると天賦を得る。複数枚使用した場合は軽減量を加算する。
/// </summary>
public class KidoDisciplineMomo : HinamoriModCard
{
    public KidoDisciplineMomo()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(Owner.Creature, "Prayer", 0f);

        // 継続的な軽減量はパワーとして保持する。
        await PowerCmd.Apply<KidoDisciplinePower>(
            choiceContext,
            Owner.Creature,
            1,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // 強化後は初手で引けるよう天賦を付与する。
        AddKeyword(CardKeyword.Innate);
    }
}
