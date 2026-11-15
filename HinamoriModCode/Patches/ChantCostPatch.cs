
using Godot;
using HarmonyLib;
using HinamoriMod.HinamoriModCode.UI;
using HinamoriMod.HinamoriModCode.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace HinamoriMod.HinamoriModCode.Patches;

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
public static class ChantCostPatch
{
    private const string NodeName = "HinamoriChantCost";

    // 既存アイコンとの重なりを避けるための移動量
    private const float IconSpacing = 55f;

    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        if (__instance.Model is not HinamoriModCard card
            || card.ChantCost < 0)
        {
            return;
        }

        var starLabel =
            __instance.GetNodeOrNull<Control>("%StarLabel");

        if (starLabel == null)
        {
            return;
        }

        // 既存の詠唱表示を取得
        var display =
            __instance.GetNodeOrNull<ChantCostDisplay>(NodeName);

        // 初回のみ生成する
        if (display == null)
        {
            display = new ChantCostDisplay
            {
                Name = NodeName
            };

            __instance.AddChild(display);

            display.Initialize(__instance);
        }

        // StarLabelのグローバル座標を
        // NCardのローカル座標に変換
        Vector2 position =
            __instance.GetGlobalTransformWithCanvas()
                .AffineInverse()
            * starLabel.GetGlobalTransformWithCanvas().Origin;

        // エンチャントアイコンが表示されているか
        bool hasEnchantment =
            __instance.EnchantmentTab.Visible;

        // 既存アイコンがある場合は詠唱を下へ移動
        if (hasEnchantment)
        {
            position += Vector2.Down * IconSpacing;
        }

        // 元のDisplayOffsetを維持する
        display.Position = position + new Vector2(25, 30);
    }
}
