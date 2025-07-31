extends CharacterBody2D
class_name Projectile

@export var HitParticle: PackedScene
@export var Hitbox: Area2D
@export var Pierce: bool
@export var LifeTime: float

var Damage: float
var Direction: Vector2
var Target: Vector2
var turret: Turret
var Targeted: Enemy

signal Hit()

func Setup(turret: Turret, Direction: Vector2, Damage: float, Target: Vector2, Targeted: Enemy) -> void:
	self.Direction = Direction
	self.Damage = Damage
	self.turret = turret
	self.Target = Target
	self.Targeted = Targeted
	Hitbox.body_entered.connect(OnBodyEntered)

func _process(delta: float) -> void:
	if LifeTime > 0:
		LifeTime -= delta
		if LifeTime <= 0:
			queue_free()
			emit_signal("Hit")

func OnBodyEntered(body: Node2D) -> void:
	if body is Enemy:
		(body as Enemy).TakeDamage(Damage)
		if HitParticle != null:
			var particle = HitParticle.instantiate() as GPUParticles2D
			particle.global_position = global_position
			particle.emitting = true
			GameManager.GM.add_child(particle)
			print("spawn")
		if not Pierce:
			emit_signal("Hit")
			queue_free()
