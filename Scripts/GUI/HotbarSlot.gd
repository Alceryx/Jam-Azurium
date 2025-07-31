extends TextureButton
class_name HotbarSlot

@export_group("Graphic")
@export var Mode1Icon: Array[Texture2D] = []
@export var Mode2Icon: Array[Texture2D] = []

@export_group("Details")
@export var icon: TextureRect
@export var amount: Label

var Amount: int
var Data: TurretData
var Mode: int

func _ready():
	BuildManager.BM.PlacedTurret.connect(_on_placement_finished)

func _process(delta: float) -> void:
	amount.text = "x%d" % Amount

func SetUp(data: TurretData, mode: int, amount: int) -> void:
	SetGraphic(mode)
	Amount = amount
	Data = data
	Mode = mode
	icon.texture = data.Modes[mode].Icon

func _pressed():
	ShopManager.SM.hide()
	BuildManager.BM.SetTurret(Data, Mode)
	BuildManager.BM.Build()

func SetGraphic(mode: int) -> void:
	match mode:
		0:
			texture_normal = Mode1Icon[0]
			texture_hover = Mode1Icon[1]
			texture_pressed = Mode1Icon[2]
		1:
			texture_normal = Mode2Icon[0]
			texture_hover = Mode2Icon[1]
			texture_pressed = Mode2Icon[2]

func _on_placement_finished(data: TurretData, mode: int) -> void:
	if data == Data and mode == Mode:
		Amount -= 1
		if Amount == 0:
			HotbarManager.HM.TurretsSlot[data].erase(mode)
			if HotbarManager.HM.TurretsSlot[data].size() == 0:
				HotbarManager.HM.TurretsSlot.erase(data)
				if HotbarManager.HM.TurretsSlot.size() == 0:
					HotbarManager.HM.hide()

			queue_free()
