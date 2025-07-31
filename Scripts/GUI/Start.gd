extends Control
class_name Start

@export var video : VideoStreamPlayer
@export var skip: Button

func _ready() -> void:
	video.show()
	video.finished.connect(video.hide)

func _on_start_pressed():
	SceneManager.SM.switch_scene("res://Scenes/World.tscn")


func OnSkipPressed() -> void:
	video.hide()
	skip.hide()
	video.stop()
