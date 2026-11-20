extends AnimatedSprite2D

# Godot-native controller: independent of C# script registration in the mod assembly.
func _ready() -> void:
    animation_finished.connect(_on_animation_finished)
    play("idle")

func _on_animation_finished() -> void:
    if animation == "attack":
        play("idle")

func _process(_delta: float) -> void:
    # Fallback if an external animator stops playback on the final attack frame.
    if animation == "attack" and not is_playing():
        play("idle")
