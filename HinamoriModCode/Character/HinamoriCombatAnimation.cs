using Godot;

namespace HinamoriMod.HinamoriModCode.Character;

/// <summary>
/// Plays a named animation on Hinamori's active combat sprite.
/// No-op outside of combat or when the visual has not been instantiated.
/// </summary>
public static class HinamoriCombatAnimation
{
    public static void Play(string animation)
    {
        if (Engine.GetMainLoop() is not SceneTree tree)
            return;

        // The combat visual is instantiated from hinamori_idle.tscn.
        // Avoid playing an animation on an unrelated character's sprite.
        foreach (Node node in tree.Root.FindChildren("IdleSprite", "AnimatedSprite2D", true, false))
        {
            if (node is not AnimatedSprite2D sprite ||
                sprite.GetParent()?.GetParent()?.Name != "HinamoriCombatVisuals" ||
                sprite.SpriteFrames == null ||
                !sprite.SpriteFrames.HasAnimation(animation))
                continue;

            sprite.Play(animation);
            return;
        }
    }
}
