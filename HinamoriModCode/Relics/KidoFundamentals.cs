
using Godot;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HinamoriMod.HinamoriModCode.Relics;

public class KidoFundamentals : HinamoriModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public bool HasReceivedNeowRelic { get; set; }

    protected virtual int StartingChant => 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Chant", StartingChant)
    ];

    /// <summary>
    /// 戦闘開始時の処理
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await PowerCmd.Apply<ChantPower>(
                null,
                Owner.Creature,
                DynamicVars["Chant"].IntValue,
                Owner.Creature,
                null
            );
        }
    }

}
