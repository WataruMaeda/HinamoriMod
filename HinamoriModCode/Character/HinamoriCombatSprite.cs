using Godot;

namespace HinamoriMod.HinamoriModCode.Character;

/// <summary>
/// 戦闘用の待機・攻撃スプライトを制御する。
/// 攻撃終了時のシグナルに加え、再生状態も監視して待機へ復帰する。
/// </summary>
public partial class HinamoriCombatSprite : AnimatedSprite2D
{
    private bool _attackStarted;

    public override void _Ready()
    {
        AnimationFinished += OnAnimationFinished;
        Play("idle");
    }

    public override void _Process(double delta)
    {
        if (Animation != "attack")
        {
            _attackStarted = false;
            return;
        }

        if (IsPlaying())
        {
            _attackStarted = true;
            return;
        }

        // 攻撃が最終フレームで停止した場合も確実に待機へ戻す。
        // シグナルが取りこぼされた場合や、外部から停止された場合の保険。
        if (_attackStarted || Frame >= SpriteFrames.GetFrameCount("attack") - 1)
            ReturnToIdle();
    }

    private void OnAnimationFinished()
    {
        if (Animation == "attack")
            ReturnToIdle();
    }

    private void ReturnToIdle()
    {
        _attackStarted = false;
        Play("idle");
    }
}
