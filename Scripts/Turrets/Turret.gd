extends StaticBody2D
class_name Turret

@export_group("References")
@export var Sprite: Sprite2D
@export var SpriteState: AnimationPlayer
@export var DetectionArea: Area2D
@export var DetectionShape: CollisionShape2D
@export var DectectionPreview: MeshInstance2D

@export_group("Visuals")
@export var HighlightShader: Shader
@export var DissolveShader: Shader
@export var DestructDelay: float
@export var HoldThreshold: float = 0.5

var DestructTimer: float
var HoldTimer: float

enum FacingDirection { TopRight, BottomRight, BottomLeft, TopLeft }

var Direction: FacingDirection = FacingDirection.TopRight
var Data: TurretData
var TargetedEnemy: Enemy
var Mode: int
var Preview: bool
var CanClick: bool
var Interacted: bool
var CanShoot: bool = true

var OccupiedPositions: Array[Vector2i] = []

func _ready():
	Sprite.material = ShaderMaterial.new()
	Sprite.material.shader = HighlightShader
	Sprite.material.set_shader_parameter("outline_color", Color(1,1,1,0))
	Sprite.material.set_shader_parameter("outline_thickness", 2.0)

	if not Preview:
		DetectionArea.body_entered.connect(OnBodyEntered)
		tree_exited.connect(OnTreeExited)

	WaveManager.WM.WaveEnded.connect(SelfDestruct)

func _process(delta):
	if DestructTimer > 0:
		Sprite.material.set_shader_parameter("dissolve_value", DestructTimer / DestructDelay)
		DestructTimer -= delta
		if DestructTimer <= 0:
			BuildManager.BM.Destroy(self)

	if Input.is_action_just_pressed("Interact") and CanClick and not BuildManager.BM.IsBuilding:
		Interacted = true
		ShowDetectionPreview()

	if Input.is_action_pressed("Interact") and CanClick and not BuildManager.BM.IsBuilding and WaveManager.WM.WaveFinished:
		HoldTimer += delta
		if HoldTimer >= HoldThreshold:
			HoldTimer = 0
			BuildManager.BM.Move(self)

	if Input.is_action_just_released("Interact"):
		HoldTimer = 0

	if (not Preview and BuildManager.BM.IsBuilding) or (Input.is_action_just_released("Interact") and not CanClick):
		Interacted = false
		HideDetectionPreview()
		var tween = create_tween()
		tween.tween_property(Sprite.material, "shader_parameter/outline_color", Color(1,1,1,0), 0.15)
		CanClick = false

	if Input.is_action_just_pressed("Destroy") and Interacted and WaveManager.WM.WaveFinished:
		BuildManager.BM.Store(self)

func SetUp(Data_: TurretData, Mode_: int, Direction_: FacingDirection, OccupiedPositions_: Array[Vector2i] = [], Preview_: bool = false):
	Data = Data_
	Mode = Mode_
	Direction = Direction_
	Preview = Preview_
	OccupiedPositions = OccupiedPositions_

	Sprite.texture = Data.Modes[Mode].Icon
	var shape = DetectionShape.shape as CircleShape2D
	shape.radius = 30 + Data.Modes[Mode].ShootRange * 70

	UpdateSprite()
	DetectionShape.disabled = Preview

func UpdateSprite():
	match Direction:
		FacingDirection.TopLeft:
			SpriteState.play("Mode %d/Top Left" % (Mode + 1))
		FacingDirection.TopRight:
			SpriteState.play("Mode %d/Top Right" % (Mode + 1))
		FacingDirection.BottomLeft:
			SpriteState.play("Mode %d/Bottom Left" % (Mode + 1))
		FacingDirection.BottomRight:
			SpriteState.play("Mode %d/Bottom Right" % (Mode + 1))

func SelfDestruct():
	if is_instance_valid(Sprite):
		var mat := Sprite.material as ShaderMaterial
		var noise := NoiseTexture2D.new()
		noise.noise = FastNoiseLite.new()
		mat.shader = DissolveShader
		mat.set_shader_parameter("dissolve_texture", noise)
		DestructTimer = DestructDelay

func ShowDetectionPreview():
	DectectionPreview.show()
	var mesh := SphereMesh.new()
	mesh.radius = 30 + Data.Modes[Mode].ShootRange * 70
	mesh.height = mesh.radius * 2
	DectectionPreview.mesh = mesh

func HideDetectionPreview():
	DectectionPreview.hide()

func OnMouseEntered():
	if Preview:
		return
	var tween = create_tween()
	tween.tween_property(Sprite.material, "shader_parameter/outline_color", Color(1,1,1), 0.15)
	CanClick = true

func OnMouseExited():
	CanClick = false
	if Preview or Interacted:
		return
	var tween = create_tween()
	tween.tween_property(Sprite.material, "shader_parameter/outline_color", Color(1,1,1,0), 0.15)

func OnBodyEntered(body: Node2D):
	if body is Enemy:
		var enemy := body as Enemy
		if CanShoot and not enemy.Targeted and not enemy.Frozen:
			TargetedEnemy = enemy
			enemy.Targeted = true
			Shoot(enemy.global_position)

func OnTreeExited():
	if is_instance_valid(TargetedEnemy):
		TargetedEnemy.Targeted = false

func Shoot(target: Vector2):
	pass # abstract
