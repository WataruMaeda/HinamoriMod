using System;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HinamoriMod.HinamoriModCode.Debug;

public partial class DebugInputHandler : Control
{
    private DebugCardMenu? _menu;

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Ignore;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo || key.Keycode != Key.F8)
            return;

        GetViewport().SetInputAsHandled();
        if (_menu != null && GodotObject.IsInstanceValid(_menu))
        {
            _menu.QueueFree();
            _menu = null;
            return;
        }

        if (!CombatManager.Instance.IsInProgress)
            return;

        var room = GetParent() as NCombatRoom;
        var player = room?.CreatureNodes.Select(c => c.Entity)
            .Where(c => c.Player != null && LocalContext.IsMe(c.Player))
            .Select(c => c.Player!)
            .FirstOrDefault();
        if (player == null)
            return;

        _menu = new DebugCardMenu(player);
        AddChild(_menu);
        _menu.Closed += () => _menu = null;
    }
}
