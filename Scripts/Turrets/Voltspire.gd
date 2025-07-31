extends Turret
class_name Voltspire

@export var ShootPoint: Marker2D
@export var Line: Line2D

func Shoot(target: Vector2) -> void:
	if not CanShoot:
		return

	var projectile: Projectile = Data.Modes[Mode].Projectile.instantiate() as Projectile

	UpdateSprite()
	projectile.Setup(self, Vector2.UP, Data.Modes[Mode].Damage, TargetedEnemy.global_position, TargetedEnemy)
	projectile.global_position = ShootPoint.global_position
	GameManager.GM.call_deferred("add_child", projectile)
	projectile.connect("Hit", Callable(self, "SelfDestruct"))

	if Mode == 0:
		projectile = Data.Modes[Mode].Projectile.instantiate() as Projectile
		projectile.Setup(self, Vector2.DOWN, Data.Modes[Mode].Damage, TargetedEnemy.global_position, TargetedEnemy)
		projectile.global_position = ShootPoint.global_position
		GameManager.GM.call_deferred("add_child", projectile)
		projectile.connect("Hit", Callable(self, "SelfDestruct"))

	Line.show()
	if Mode == 0:
		SpriteState.play("Mode 1 Shoot")
	else:
		SpriteState.play("Mode 2 Shoot")

	CanShoot = false

func UpdateSprite() -> void:
	if Mode == 0:
		SpriteState.play("Mode 1")
	else:
		SpriteState.play("Mode 2")
