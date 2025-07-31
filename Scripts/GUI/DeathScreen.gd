extends Control
class_name DeathScreen

@export var wave_survived: Label

func _ready():
	process_mode = Node.PROCESS_MODE_ALWAYS
	hide()

func _process(delta):
	if GameManager.GM.ButtonHP <= 0:
		get_tree().paused = true
		wave_survived.text = str(WaveManager.WM.CurrentWaveNumber - 1)
		show()

func on_replay_pressed():
	get_tree().paused = false
	get_tree().reload_current_scene()

func on_quit_pressed():
	SceneManager.SM.switch_scene("res://Scenes/GUI/Start.tscn")
	get_tree().paused = false
	queue_free()
