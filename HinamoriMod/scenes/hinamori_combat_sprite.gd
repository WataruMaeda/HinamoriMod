extends AnimatedSprite2D

# Combat animations return to idle after their one-shot playback.
const ONE_SHOT_ANIMATIONS := [&"attack", &"block", &"cast"]

func _ready() -> void:
    animation_finished.connect(_on_animation_finished)
    play("idle")

func _on_animation_finished() -> void:
    if animation in ONE_SHOT_ANIMATIONS:
        play("idle")

func _process(_delta: float) -> void:
    # Also handle animation playback stopped externally on the last frame.
    if animation in ONE_SHOT_ANIMATIONS and not is_playing():
        play("idle")
