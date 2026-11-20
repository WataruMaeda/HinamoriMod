using Godot;

namespace HinamoriMod.HinamoriModCode.Character;

/// <summary>
/// 戦闘用スプライト。攻撃アニメーションの終了後、待機ループに戻す。
/// BaseLib の CustomAnimationPatch が AnimatedSprite2D の attack を再生する。
/// </summary>
public partial class HinamoriCombatSprite : AnimatedSprite2D
{
    public override void _Ready()
    {
        AnimationFinished += OnAnimationFinished;
        Play("idle");
    }

    private void OnAnimationFinished()
    {
        if (Animation == "attack")
            Play("idle");
    }
}
