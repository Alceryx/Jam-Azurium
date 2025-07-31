extends Control

@export var health: Label
@export var health_bar: TextureProgressBar
@export var efficiency: RichTextLabel
@export var currency: Label

func _physics_process(delta: float) -> void:
	if GameManager.GM:
		health_bar.value = GameManager.GM.ButtonHP
		health_bar.max_value = GameManager.GM.ButtonMaxHP

	if (GameManager.GM.ButtonHP >= 0):
		health.text = "%d/%d" % [GameManager.GM.ButtonHP, GameManager.GM.ButtonMaxHP]
	else: health.text = "0/0"
	currency.text = str(GameManager.GM.Currency)
	efficiency.text = "%d / click" % GameManager.GM.Efficiency
