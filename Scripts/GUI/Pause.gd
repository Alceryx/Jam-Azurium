extends Control
class_name Pause

@export var Fullscreen: TextureButton

func _ready():
	get_tree().paused = true
	process_mode = Node.PROCESS_MODE_ALWAYS
	Fullscreen.button_pressed = DisplayServer.window_get_mode() == DisplayServer.WINDOW_MODE_FULLSCREEN

func _process(delta):
	if Input.is_action_just_pressed("Escape"):
		_on_continue_pressed()

func _on_continue_pressed():
	get_tree().paused = false
	queue_free()

func _on_quit_pressed():
	SceneManager.SM.SwitchScene("res://Scenes/GUI/Start.tscn")
	get_tree().paused = false
	queue_free()

func _on_button_toggled(toggled_on: bool):
	if toggled_on:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
