extends PanelContainer
class_name Upgrade

@export_group("Data")
@export var Database: Array[UpgradeData] = []

@export_group("Graphic")
@export var Background: TextureRect
@export var BGDefault: Texture2D
@export var BGSelected: Texture2D
@export var Tier: TextureRect
@export var CostBar: HBoxContainer

var SelectHover: Texture2D
var SelectPressed: Texture2D

@export_group("Detail")
@export var Info: Label
@export var Price: Label

@export_group("Dynamic")
@export var ItemPanel: PanelContainer
@export var Lock: TextureRect

@export_group("Button")
@export var Select: TextureButton

var CurrentTier: int = 0
var Affordable: bool
var IsSelected: bool = false
var Maxed: bool:
	get:
		return CurrentTier == Database.size()

func _ready():
	SelectHover = Select.texture_hover
	SelectPressed = Select.texture_pressed

	Select.pressed.connect(OnSelection)
	Lock.hide()

func _process(delta: float) -> void:
	if not Maxed:
		if GameManager.GM and GameManager.GM.Currency < Database[CurrentTier].Price:
			Price.add_theme_color_override("font_color", Color.html("A94241"))
			Affordable = false
		else:
			Price.add_theme_color_override("font_color", Color.WHITE)
			Affordable = true
	else:
		CostBar.hide()

	if ShopManager.SM.ActiveUpgrade != self or ShopManager.SM.SelectedUpgrade == null:
		Deselect()

	Price.text = "" if Maxed else str(Database[CurrentTier].Price)

func Update():
	Tier.texture = Database[CurrentTier].Graphic
	if CurrentTier < Database.size():
		CurrentTier += 1

func OnSelection():
	IsSelected = not IsSelected
	Background.texture = BGSelected if IsSelected else BGDefault

	if IsSelected:
		DisableHoverGraphic()
	else:
		EnableHoverGraphic()

	ShopManager.SM.SelectedUpgrade = self if IsSelected else null
	ShopManager.SM.AnySelected = IsSelected
	ShopManager.SM.ActiveUpgrade = self

func DisableHoverGraphic():
	Select.texture_hover = null
	Select.texture_pressed = null

func EnableHoverGraphic():
	Select.texture_hover = SelectHover
	Select.texture_pressed = SelectPressed

func Deselect():
	IsSelected = false
	Background.texture = BGDefault
