
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch]
public static class NeowRelicPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.DeclaredMethod(
            typeof(AncientEventModel),
            "BeforeEventStarted"
        );
    }

    private static void Postfix(
        AncientEventModel __instance,
        bool isPreFinished,
        ref Task __result)
    {
        if (__instance is not Neow || isPreFinished)
            return;

        __result = GiveRelicAfterOriginal(__result, __instance);
    }

    private static async Task GiveRelicAfterOriginal(
        Task originalTask,
        AncientEventModel ancient)
    {
        // ネオー本来の開始処理を待つ
        await originalTask;

        var player = ancient.Owner;
        if (player == null)
            return;

        // 鬼道の心得を所持しているか確認
        var starterRelic = player.GetRelic<KidoFundamentals>();
        if (starterRelic == null)
            return;

        // 既に獲得済みなら何もしない
        if (starterRelic.HasReceivedNeowRelic)
            return;

        // ランダムなレリックを抽選
        var relic = RelicFactory
            .PullNextRelicForRewardFromFront(player)
            .ToMutable();

        // 追加レリックを直接獲得
        await RelicCmd.Obtain(relic, player);

        // 獲得済みとして記録
        starterRelic.HasReceivedNeowRelic = true;
    }
}
