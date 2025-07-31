extends Control
class_name ShopManager

@export_group("List")
@export var ItemContainer: VBoxContainer
@export var UpgradeList: VBoxContainer

@export_group("Item")
@export var Database: Array[TurretData] = []
@export var ItemFrame: PackedScene

@export_group("Tab")
@export var DefenceTab: TextureButton
@export var UpgradeTab: TextureButton
@export var DefenceScreen: PanelContainer
@export var UpgradeScreen: PanelContainer

@export_group("Button")
@export var Buy: TextureButton

static var SM: ShopManager

var AnySelected: bool
var SelectedItem: ShopItem
var SelectedUpgrade: Upgrade
var Mode: ButtonGroup = ButtonGroup.new()
var ActiveUpgrade: Upgrade
var SignalConnected: bool
var IsInArea: bool

func _ready():
	SM = self
	Buy.disabled = true
	UpgradeScreen.hide()
	Mode.allow_unpress = true

	var tab: ButtonGroup = ButtonGroup.new()
	DefenceTab.button_group = tab
	UpgradeTab.button_group = tab

	DisplayItem()

func _process(delta: float) -> void:
	if IsInArea:
		GameManager.GM.NexusButton.CanClick = false

	if not SignalConnected:
		WaveManager.WM.connect("WaveEnded", Callable(self, "Unlock"))
		WaveManager.WM.connect("WaveStarted", Callable(self, "hide"))
		SignalConnected = true

	if Input.is_action_just_pressed("Escape"):
		hide()

	if Affordable():
		Buy.disabled = not AnySelected
	else:
		Buy.disabled = true

func OnPurchase():
	if SelectedItem and GameManager.GM.Currency >= SelectedItem.Price:
		LockItem()

		if HotbarManager.HM.TurretsSlot.has(SelectedItem.Data):
			if HotbarManager.HM.TurretsSlot[SelectedItem.Data].has(SelectedItem.Mode):
				HotbarManager.HM.UpdateSlot(SelectedItem.Data, SelectedItem.Mode)
			else:
				HotbarManager.HM.AddSlot(SelectedItem.Data, SelectedItem.Mode)
		else:
			HotbarManager.HM.AddSlot(SelectedItem.Data, SelectedItem.Mode)

		GameManager.GM.Currency -= SelectedItem.Price

	elif SelectedUpgrade and GameManager.GM.Currency >= SelectedUpgrade.Database[SelectedUpgrade.CurrentTier].Price:
		LockUpgrade()
		GameManager.GM.Currency -= SelectedUpgrade.Database[SelectedUpgrade.CurrentTier].Price
		SelectedUpgrade.Update()

func Affordable() -> bool:
	return (SelectedItem and SelectedItem.Affordable) or (SelectedUpgrade and SelectedUpgrade.Affordable and not SelectedUpgrade.Maxed)

func LockItem():
	for item : ShopItem in ItemContainer.get_children():
		if item != SelectedItem:
			item.ItemPanel.hide()
			item.Lock.show()

func LockUpgrade():
	for upgrade : Upgrade in UpgradeList.get_children():
		if upgrade != SelectedUpgrade:
			upgrade.ItemPanel.hide()
			upgrade.Lock.show()

func Unlock():
	for upgrade : Upgrade in UpgradeList.get_children():
		upgrade.ItemPanel.show()
		upgrade.Lock.hide()

	PackUp()
	DisplayItem()

func OnClosePressed():
	hide()

func DisplayItem():
	for data in Database:
		var item: ShopItem = ItemFrame.instantiate() as ShopItem
		item.SetUp(data, 0)
		ItemContainer.add_child(item)

func PackUp():
	for item in ItemContainer.get_children():
		item.queue_free()

func OnDefenceToggled(toggled_on: bool):
	DefenceScreen.visible = toggled_on
	SelectedItem = null
	SelectedUpgrade = null
	AnySelected = false

func OnUpgradeToggled(toggled_on: bool):
	UpgradeScreen.visible = toggled_on
	SelectedItem = null
	SelectedUpgrade = null
	AnySelected = false

func OnMouseEntered():
	IsInArea = true

func OnMouseExited():
	IsInArea = false
