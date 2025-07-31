extends Node
class_name BuildManager

@export_group("Data")
@export var TurretInfo: TurretData
@export var Mode: int = 1

@export_group("References")
@export var PreviewLayer: TileMapLayer
@export var PlaceableLayer: TileMapLayer
@export var GroundLayer: Array[TileMapLayer] = []
@export var UnplaceableLayer: Array[TileMapLayer] = []

var CurrentPreview: Turret
var PreviewRotation: Turret.FacingDirection = Turret.FacingDirection.TopRight

var IsBuilding: bool = false
var IsRelocating: bool = false

var OccupiedTiles: Array[Vector2i] = []
var RecentlyOccupied: Array[Vector2i] = []

static var BM: BuildManager

var PrevPos: Vector2i
var GridPos: Vector2i

signal PlacedTurret(data: TurretData, mode: int)

func _ready():
	BM = self
	PreviewLayer.child_entered_tree.connect(_update_preview_data)
	PlaceableLayer.child_entered_tree.connect(_update_placement_data)

func _process(delta: float):
	if IsBuilding:
		GridPos = PreviewLayer.local_to_map(PreviewLayer.get_local_mouse_position())

	if IsPlacementValid():
		PreviewLayer.modulate = Color(1, 1, 1, 0.5)
	else:
		PreviewLayer.modulate = Color(1, 0, 0, 0.75)

	if Input.is_action_just_released("Interact") and IsBuilding and IsPlacementValid():
		Place()

	if Input.is_action_just_pressed("Rotate") and TurretInfo.Modes[Mode].CanRotate and IsBuilding:
		Rotate()

	if Input.is_action_just_pressed("Escape") and IsBuilding:
		CancelPlacement()

	if IsBuilding:
		PreviewPlacement()

func SetTurret(turret_info: TurretData, mode: int) -> void:
	TurretInfo = turret_info
	Mode = mode

func Build():
	IsBuilding = true
	PreviewRotation = Turret.FacingDirection.TopRight

func Destroy(turret: Turret) -> void:
	for pos in turret.OccupiedPositions:
		OccupiedTiles.erase(pos + Vector2i.ONE)
	PlaceableLayer.erase_cell(turret.OccupiedPositions[0])

func CancelPlacement():
	if IsRelocating:
		Place()
	else:
		PreviewLayer.erase_cell(GridPos)
		IsBuilding = false

func Store(turret: Turret) -> void:
	Destroy(turret)
	if HotbarManager.HM.TurretsSlot.has(turret.Data):
		if HotbarManager.HM.TurretsSlot[turret.Data].has(turret.Mode):
			HotbarManager.HM.UpdateSlot(turret.Data, turret.Mode)
		else:
			HotbarManager.HM.AddSlot(turret.Data, turret.Mode)
	else:
		HotbarManager.HM.AddSlot(turret.Data, turret.Mode)

func Move(turret: Turret) -> void:
	Destroy(turret)
	SetTurret(turret.Data, turret.Mode)
	Build()
	IsRelocating = true

func Rotate() -> void:
	PreviewRotation += 1
	if PreviewRotation > 3:
		PreviewRotation = 0
	
	_update_preview_data(CurrentPreview)

func Place():
	IsRelocating = false
	var RealPos = GridPos + Vector2i.ONE
	IsBuilding = false
	PreviewLayer.erase_cell(GridPos)
	PlaceableLayer.set_cell(GridPos, 2, Vector2i.ZERO, TurretInfo.ID)
	RecentlyOccupied.clear()

	for x in TurretInfo.Size.x:
		for y in TurretInfo.Size.y:
			var pos: Vector2i
			var display: Vector2i

			match PreviewRotation:
				Turret.FacingDirection.TopRight:
					pos = RealPos + Vector2i(x, -y)
					display = GridPos + Vector2i(x, -y)
				Turret.FacingDirection.TopLeft:
					pos = RealPos + Vector2i(-y, x)
					display = GridPos + Vector2i(-y, x)
				Turret.FacingDirection.BottomRight:
					pos = RealPos + Vector2i(y, x)
					display = GridPos + Vector2i(y, x)
				_:
					pos = RealPos + Vector2i(x, y)
					display = GridPos + Vector2i(x, y)

			OccupiedTiles.append(pos)
			RecentlyOccupied.append(display)

func IsPlacementValid() -> bool:
	var RealPos = GridPos + Vector2i.ONE

	for x in TurretInfo.Size.x:
		for y in TurretInfo.Size.y:
			var pos: Vector2i
			match PreviewRotation:
				Turret.FacingDirection.TopRight:
					pos = RealPos + Vector2i(x, -y)
				Turret.FacingDirection.TopLeft:
					pos = RealPos + Vector2i(-y, x)
				Turret.FacingDirection.BottomRight:
					pos = RealPos + Vector2i(y, x)
				_:
					pos = RealPos + Vector2i(x, y)

			if OccupiedTiles.has(pos):
				return false

			for layer in UnplaceableLayer:
				if is_instance_valid(layer.get_cell_tile_data(pos)):
					return false

			for layer in GroundLayer:
				var tile = layer.get_cell_tile_data(pos)
				if is_instance_valid(tile) and tile.get_custom_data("Placeable"):
					return true

			return false

	return true

func PreviewPlacement():
	if PrevPos != GridPos:
		PreviewLayer.erase_cell(PrevPos)

	PreviewLayer.set_cell(GridPos, 2, Vector2i.ZERO, TurretInfo.ID)
	PrevPos = GridPos

func _update_preview_data(node: Node):
	if node is Turret:
		CurrentPreview = node as Turret
		if not is_instance_valid(CurrentPreview.Sprite):
			return
		CurrentPreview.SetUp(TurretInfo, Mode, PreviewRotation, [], true)
		CurrentPreview.ShowDetectionPreview()

func _update_placement_data(node: Node):
	if node is Turret:
		var turret = node as Turret
		var temp: Array[Vector2i] = RecentlyOccupied.duplicate()
		turret.SetUp(TurretInfo, Mode, PreviewRotation, temp)
		turret.HideDetectionPreview()
		emit_signal("PlacedTurret", TurretInfo, Mode)
