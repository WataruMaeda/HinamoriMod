
using System.Threading.Tasks;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch(typeof(AbstractRoom), nameof(AbstractRoom.Enter))]
public static class StartingRelicFallbackPatch
{
    [HarmonyPostfix]
    private static void Postfix(
        AbstractRoom __instance,
        IRunState? runState,
        bool isRestoringRoomStackBase,
        ref Task __result)
    {
        if (runState == null || isRestoringRoomStackBase)
            return;

        // ネオーの場合は既存Patchに任せる
        if (__instance is EventRoom eventRoom &&
            eventRoom.CanonicalEvent is Neow)
            return;

        // 最初のActだけを対象とする
        if (runState.CurrentActIndex != 0)
            return;

        __result = GiveRelicAfterRoomEntered(__result, runState);
    }

    private static async Task GiveRelicAfterRoomEntered(
        Task originalTask,
        IRunState runState)
    {
        // 本来の入室処理を完了させる
        await originalTask;

        foreach (var player in runState.Players)
        {
            var starterRelic =
                player.GetRelic<KidoFundamentals>();

            if (starterRelic == null)
                continue;

            // 既に追加レリックを獲得している場合は何もしない
            if (starterRelic.HasReceivedNeowRelic)
                continue;

            var relic = RelicFactory
                .PullNextRelicForRewardFromFront(player)
                .ToMutable();

            await RelicCmd.Obtain(relic, player);

            starterRelic.HasReceivedNeowRelic = true;
        }
    }
}
