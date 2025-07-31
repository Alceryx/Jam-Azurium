extends PanelContainer
class_name ShopItem

@export_group("Dynamic")
@export var ItemPanel: PanelContainer
@export var Lock: TextureRect
@export var Icon: TextureRect
@export var ModeButton: Array[TextureButton] = []

@export_group("Detail")
@export var NameLabel: Label
@export var PriceLabel: Label
@export var DescriptionLabel: RichTextLabel

var Affordable: bool
var Price: int

var ModeCount: int
var Mode: int
var Data: TurretData

func _ready():
	Lock.hide()
	for button in ModeButton:
		button.button_group = ShopManager.SM.Mode

func _process(delta: float) -> void:
	if not ShopManager.SM.DefenceScreen.visible:
		Deselect()

	if GameManager.GM != null and GameManager.GM.Currency < Data.Modes[Mode].Price:
		PriceLabel.add_theme_color_override("font_color", Color.html("A94241"))
		Affordable = false
	else:
		PriceLabel.add_theme_color_override("font_color", Color.WHITE)
		Affordable = true

func SetUp(data: TurretData, mode: int) -> void:
	Price = data.Modes[mode].Price
	ModeCount = data.Modes.size()
	Mode = mode
	Data = data

	NameLabel.text = data.Name
	DescriptionLabel.text = data.Modes[mode].Description
	Icon.texture = data.Modes[mode].Icon
	PriceLabel.text = str(data.Modes[mode].Price)

func Deselect():
	for button in ModeButton:
		button.button_pressed = false

func OnMode1Selected(toggled_on: bool) -> void:
	SetUp(Data, 0)
	ShopManager.SM.SelectedItem = self if toggled_on else null
	ShopManager.SM.AnySelected = toggled_on

func OnMode2Selected(toggled_on: bool) -> void:
	SetUp(Data, 1)
	ShopManager.SM.SelectedItem = self if toggled_on else null
	ShopManager.SM.AnySelected = toggled_on
