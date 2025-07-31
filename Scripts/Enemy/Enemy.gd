# Enemy.gd
extends CharacterBody2D
class_name Enemy

enum FacingDirection {
	TopLeft,
	TopRight,
	BottomLeft,
	BottomRight
}

@export_group("Data")
@export var MaxHealth: float = 5.0
@export var Speed: float = 100.0
@export var Damage: float = 5.0
var CurrentHealth: float

@export_group("References")
@export var sprite: AnimationPlayer

var Path: Path2D
var PathFollow: PathFollow2D
var LastPosition: Vector2
var MoveDirection: Vector2
var Direction: FacingDirection

var Targeted: bool
var Frozen: bool

func Setup(Path: Path2D) -> void:
	self.Path = Path

	PathFollow = PathFollow2D.new()
	PathFollow.progress = 0
	PathFollow.loop = false
	PathFollow.rotates = false
	Path.add_child(PathFollow)

	global_position = PathFollow.global_position - Vector2(0, 64)

func _ready() -> void:
	CurrentHealth = MaxHealth
	WaveManager.WM.WaveStarted.connect(UnFreeze)

func TakeDamage(damage: float) -> void:
	CurrentHealth -= damage
	print(CurrentHealth)
	if CurrentHealth <= 0:
		Die()

func Freeze() -> void:
	Frozen = true
	modulate = Color(0, 0, 1)
	remove_from_group("Enemy")

func UnFreeze() -> void:
	if Frozen and is_instance_valid(self):
		Frozen = false
		modulate = Color(1, 1, 1)
		add_to_group("Enemy")

func _physics_process(delta: float) -> void:
	if not is_instance_valid(Path) or not is_instance_valid(PathFollow) or Frozen:
		return

	PathFollow.progress += Speed * delta
	global_position = PathFollow.global_position - Vector2(0, 64)
	UpdateFacingDirection()

func UpdateFacingDirection() -> void:
	var angle =  Vec2Extensions.get_isometric_angle_to(LastPosition, global_position)
	MoveDirection = (global_position - LastPosition).normalized()
	LastPosition = global_position

	if angle >= 225 and angle < 315:
		Direction = FacingDirection.TopLeft
	elif angle >= 315 or angle < 45:
		Direction = FacingDirection.TopRight
	elif angle >= 135 and angle < 225:
		Direction = FacingDirection.BottomLeft
	elif angle >= 45 and angle < 135:
		Direction = FacingDirection.BottomRight

	UpdateSprite()

func CartesianToIsometric(vector: Vector2) -> Vector2:
	return Vector2(vector.x - vector.y, (vector.x + vector.y) / 2.0)

func UpdateSprite() -> void:
	match Direction:
		FacingDirection.TopLeft:
			sprite.play("Top Left")
		FacingDirection.TopRight:
			sprite.play("Top Right")
		FacingDirection.BottomLeft:
			sprite.play("Bottom Left")
		FacingDirection.BottomRight:
			sprite.play("Bottom Right")
		

func Die():
	WaveManager.WM.WaveStarted.disconnect(UnFreeze)
	queue_free()
