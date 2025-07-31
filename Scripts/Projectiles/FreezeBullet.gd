extends Projectile

@export_group("Info")
@export var Speed: float
@export var HoverTime: float
@export var HoverHeight: float

@export_group("Freeze")
@export var FreezeArea: CollisionShape2D

var EnemyToFreeze: Array = []
var CanFreeze: bool = false
var HoverTimer: float
var Hovered: bool = false
var OriginalPosition: Vector2

func _ready() -> void:
	HoverTimer = HoverTime
	OriginalPosition = global_position
	WaveManager.WM.WaveStarted.connect(Destroy)

func _physics_process(delta: float) -> void:
	if CanFreeze:
		for enemy in EnemyToFreeze:
			if not enemy.Frozen:
				enemy.Freeze()
		FreezeArea.disabled = true
		return

	if HoverTimer > 0:
		HoverTimer -= delta
		if not Hovered:
			velocity = Vector2.UP * Speed
			if global_position.distance_to(OriginalPosition) >= HoverHeight:
				if is_instance_valid(Targeted):
					if Direction == Vector2.UP:
						Direction = (Targeted.global_position - global_position).normalized()
					else:
						Direction = (global_position - Targeted.global_position).normalized()
				Hovered = true
		else:
			velocity = Vector2.ZERO
	else:
		velocity = Direction * Speed
		look_at(Direction * 10000)

	move_and_slide()

func OnBodyEntered(body: Node2D) -> void:
	if body is Enemy and not CanFreeze:
		if HitParticle != null:
			var particle = HitParticle.instantiate() as GPUParticles2D
			particle.global_position = global_position
			particle.emitting = true
			GameManager.GM.add_child(particle)
			print("spawn")
		(body as Enemy).TakeDamage(Damage)
		emit_signal("Hit")
		CanFreeze = true

func OnFreezeEntered(body: Node2D) -> void:
	if body is Enemy:
		EnemyToFreeze.append(body)

func OnFreezeExited(body: Node2D) -> void:
	if body is Enemy:
		EnemyToFreeze.erase(body)

func Destroy() -> void:
	if is_instance_valid(self):
		WaveManager.WM.WaveStarted.disconnect(Destroy)
		queue_free()
