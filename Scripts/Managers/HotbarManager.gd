extends Control
class_name HotbarManager

@export var Bar: HBoxContainer
@export var Slot: PackedScene

var TurretsSlot: Dictionary = {}
static var HM: HotbarManager

var SignalConnected: bool = false

func _ready():
	HM = self

func _process(delta: float) -> void:
	if not SignalConnected:
		WaveManager.WM.WaveStarted.connect(hide)
		WaveManager.WM.WaveEnded.connect(show)
		SignalConnected = true

func AddSlot(data: TurretData, mode: int) -> void:
	var slot: HotbarSlot = Slot.instantiate() as HotbarSlot

	if not visible:
		show()

	if TurretsSlot.has(data):
		TurretsSlot[data][mode] = slot
	else:
		TurretsSlot[data] = {mode: slot}

	slot.SetUp(data, mode, 1)
	Bar.add_child(slot)

func UpdateSlot(data: TurretData, mode: int) -> void:
	TurretsSlot[data][mode].SetUp(data, mode, TurretsSlot[data][mode].Amount + 1)

func Appear():
	if TurretsSlot.size() > 0:
		show()
