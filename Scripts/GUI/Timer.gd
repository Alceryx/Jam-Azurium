extends Control

@export var background: TextureRect
@export var time_expiring: Texture2D
@export var time: Label
@export var wave: Label

var bg_default: Texture2D

func _ready() -> void:
	bg_default = background.texture

func _process(delta: float) -> void:
	if WaveManager.WM:
		time.text = "00.00" if WaveManager.WM.WaveTimer <= 0 else "%.2f" % WaveManager.WM.WaveTimer
		if (WaveManager.WM.WaveTimer < 10 && WaveManager.WM.WaveTimer > 0):
			time.text = "0" + time.text
		wave.text = "%d/%d" % [WaveManager.WM.CurrentWaveNumber, WaveManager.WM.Waves.size()]

		if WaveManager.WM.WaveTimer <= 5:
			end_soon()
		else:
			default_state()

func default_state() -> void:
	background.texture = bg_default
	time.add_theme_color_override("font_color", Color.html("73DEE2"))
	wave.add_theme_color_override("font_color", Color.html("2b4f6f"))

func end_soon() -> void:
	background.texture = time_expiring
	time.add_theme_color_override("font_color", Color.html("D22424"))
	wave.add_theme_color_override("font_color", Color.html("7A0000"))
