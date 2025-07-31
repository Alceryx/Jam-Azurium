extends Control
class_name Recess

@export var count_down: Label

func _ready():
	hide()

func _process(delta):
	if not WaveManager.WM.CountDownFinished:
		show()
		if WaveManager.WM.CountDownTimer <= 1:
			count_down.text = "Wave %d" % WaveManager.WM.CurrentWaveNumber
		else:
			count_down.text = str(int(WaveManager.WM.CountDownTimer))
	else:
		hide()
