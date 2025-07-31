extends Control
class_name Start

@export var video : VideoStreamPlayer
@export var skip: Button
@export var pause : PackedScene

func _ready() -> void:
	video.show()
	video.finished.connect(video.hide)

func _process(delta: float) -> void:
	if (Input.is_action_just_pressed("Escape")):
		var p : Pause = pause.instantiate() as Pause
		add_child(p)

func _on_start_pressed():
	SceneManager.SM.switch_scene("res://Scenes/World.tscn")


func OnSkipPressed() -> void:
	video.hide()
	skip.hide()
	video.stop()
