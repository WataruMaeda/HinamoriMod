using Godot;

namespace HinamoriMod.HinamoriModCode.UI;

public partial class ResourceCostDisplay : Control
{
    private TextureRect? _icon;
    private Label? _costLabel;

    // 継承先で変更できる設定
    protected virtual string IconPath => "";

    protected virtual Vector2 DisplayOffset =>
        new Vector2(25, 30);

    protected virtual Vector2 IconPosition =>
        new Vector2(-20, -16);

    protected virtual Vector2 IconSize =>
        new Vector2(64, 64);

    protected virtual Vector2 LabelPosition =>
        new Vector2(-7, -5);

    protected virtual Vector2 LabelSize =>
        new Vector2(38, 38);

    protected virtual int FontSize => 25;

    public void Initialize()
    {
        Position += DisplayOffset;

        MouseFilter = MouseFilterEnum.Ignore;
        ZIndex = 0;

        _icon = new TextureRect
        {
            Position = IconPosition,
            Size = IconSize,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = MouseFilterEnum.Ignore
        };

        _icon.Texture = ResourceLoader.Load<Texture2D>(IconPath);
        AddChild(_icon);

        _costLabel = new Label
        {
            Position = LabelPosition,
            Size = LabelSize,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore
        };

        _costLabel.AddThemeFontSizeOverride(
            "font_size", FontSize
        );

        _costLabel.AddThemeColorOverride(
            "font_outline_color", Colors.Black
        );

        _costLabel.AddThemeConstantOverride(
            "outline_size", 5
        );

        AddChild(_costLabel);
    }

    protected void SetCost(int cost, bool insufficient)
    {
        if (_costLabel == null)
            return;

        _costLabel.Text = cost.ToString();

        _costLabel.AddThemeColorOverride(
            "font_color",
            insufficient ? Colors.Red : Colors.White
        );
    }
}