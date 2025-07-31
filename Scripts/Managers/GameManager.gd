extends Node
class_name GameManager

static var GM: GameManager

@export_group("Nexus Button")
@export var Efficiency: int
@export var ButtonMaxHP: float
var ButtonHP: float

@export_group("Misc")
@export var Audio: AudioStreamPlayer
@export var AudioDelay: float
@export var PauseMenu: PackedScene

var AudioDelayTimer: float
var Currency: int
var NexusButton: NexusButton

func _ready():
	GM = self
	ButtonHP = ButtonMaxHP
	AudioDelayTimer = AudioDelay

func _process(delta: float) -> void:
	if AudioDelayTimer > 0:
		AudioDelayTimer -= delta
		if AudioDelayTimer <= 0:
			Audio.play()

	if Input.is_action_just_pressed("Escape") and not ShopManager.SM.visible and not BuildManager.BM.IsBuilding:
		var pause: Pause = PauseMenu.instantiate() as Pause
		get_tree().root.add_child(pause)
