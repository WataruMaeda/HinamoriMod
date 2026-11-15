
using Godot;
using HinamoriMod.HinamoriModCode.Cards;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace HinamoriMod.HinamoriModCode.UI;

public partial class ChantCostDisplay : ResourceCostDisplay
{
    private NCard? _card;

    protected override string IconPath =>
        "res://HinamoriMod/images/icons/chant_icon.png";

    public void Initialize(NCard card)
    {
        _card = card;
        base.Initialize();
        UpdateDisplay();
    }

    public override void _Process(double delta)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_card?.Model is not HinamoriModCard card
            || card.ChantCost < 0)
        {
            Visible = false;
            return;
        }

        Visible = true;

        // 戦闘外では通常の詠唱コストを表示
        if (!card.IsMutable || card.CombatState == null)
        {
            SetCost(card.ChantCost, false);
            return;
        }

        // 黄火閃などの軽減を反映した詠唱コスト
        int effectiveCost = card.EffectiveChantCost;

        int currentChant = (int)(
            card.Owner.Creature
                .GetPower<ChantPower>()?.Amount ?? 0
        );

        // 詠唱不足なら赤色表示
        SetCost(
            effectiveCost,
            currentChant < effectiveCost
        );
    }
}
