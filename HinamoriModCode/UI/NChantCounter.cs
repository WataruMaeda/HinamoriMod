using Godot;
using HinamoriMod.HinamoriModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Players;

namespace HinamoriMod.HinamoriModCode.UI;

public partial class NChantCounter : Control
{
    private Player? _player;
    private Label? _label;
    private int _lastAmount = -1;

    public void Initialize(Player player)
    {
        _player = player;

        MouseFilter = MouseFilterEnum.Ignore;

        // 詠唱アイコン
        var icon = new TextureRect
        {
            Position = new Vector2(-32, -32),
            Size = new Vector2(64, 64),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = MouseFilterEnum.Ignore,
            Texture = ResourceLoader.Load<Texture2D>(
                "res://HinamoriMod/images/icons/chant_icon.png")
        };

        AddChild(icon);

        // 詠唱数
        _label = new Label
        {
            Position = new Vector2(-20, -18),
            Size = new Vector2(40, 36),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore
        };

        _label.AddThemeFontSizeOverride("font_size", 24);
        _label.AddThemeColorOverride("font_color", Colors.White);
        _label.AddThemeColorOverride("font_outline_color", Colors.Black);
        _label.AddThemeConstantOverride("outline_size", 5);

        AddChild(_label);

        RefreshAmount();
    }

    public override void _Process(double delta)
    {
        RefreshAmount();
    }

    private void RefreshAmount()
    {
        if (_player == null || _label == null)
            return;

        int amount = (int)(
            _player.Creature.GetPower<ChantPower>()?.Amount ?? 0
        );

        if (amount == _lastAmount)
            return;

        _lastAmount = amount;
        _label.Text = amount.ToString();
    }
}