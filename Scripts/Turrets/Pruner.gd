extends Turret
class_name Pruner

@export var OriginPoint: Marker2D
@export var TopLeftShootPoint: Marker2D
@export var TopRightShootPoint: Marker2D
@export var BottomLeftShootPoint: Marker2D
@export var BottomRightShootPoint: Marker2D

var DirCast: Array[RayCast2D] = []
var EnemyEnteredLaser: int = 0

func _ready():
	super._ready()

	if not Preview and Mode == 1:
		Shoot(Vector2.ZERO)

	if not Preview and Mode == 0:
		DetectionShape.disabled = true
		var ShootPoints = [
			TopLeftShootPoint.global_position,
			TopRightShootPoint.global_position,
			BottomLeftShootPoint.global_position,
			BottomRightShootPoint.global_position
		]

		for point in ShootPoints:
			var ray = RayCast2D.new()
			add_child(ray)
			ray.global_position = OriginPoint.global_position + Vector2(0, 50)
			ray.target_position = (point - OriginPoint.global_position).normalized() * (30 + Data.Modes[Mode].ShootRange * 70)
			DirCast.append(ray)

func _process(delta):
	if Mode == 0:
		for ray in DirCast:
			if ray.get_collider() is Enemy:
				var enemy = ray.get_collider() as Enemy
				if CanShoot and not enemy.Targeted and not enemy.Frozen:
					TargetedEnemy = enemy
					enemy.Targeted = true
					Shoot(ray.get_collision_point())
	super._process(delta)

func Shoot(target: Vector2):
	if not CanShoot:
		return

	if Mode == 0:
		var projectile = Data.Modes[Mode].Projectile.instantiate() as Projectile
		var angle = Vec2Extensions.get_isometric_angle_to(global_position, target)
		var ShootPoint: Vector2

		if angle >= 225 and angle < 315:
			Direction = FacingDirection.TopLeft
			ShootPoint = TopLeftShootPoint.global_position
		elif angle >= 315 or angle < 45:
			Direction = FacingDirection.TopRight
			ShootPoint = TopRightShootPoint.global_position
		elif angle >= 135 and angle < 225:
			Direction = FacingDirection.BottomLeft
			ShootPoint = BottomLeftShootPoint.global_position
		elif angle >= 45 and angle < 135:
			Direction = FacingDirection.BottomRight
			ShootPoint = BottomRightShootPoint.global_position

		UpdateSprite()

		var direction = (target - ShootPoint).normalized()
		projectile.Setup(self, direction, Data.Modes[Mode].Damage, target, TargetedEnemy)
		projectile.global_position = ShootPoint
		projectile.Hit.connect(OnProjectileHit)
		GameManager.GM.call_deferred("add_child", projectile)
	else:
		var projectile = Data.Modes[Mode].Projectile.instantiate() as Projectile
		projectile.Hitbox.body_entered.connect(OnLaserEntered)

		var ShootPoint: Vector2
		match Direction:
			FacingDirection.TopLeft:
				ShootPoint = TopLeftShootPoint.global_position
			FacingDirection.TopRight:
				ShootPoint = TopRightShootPoint.global_position
			FacingDirection.BottomLeft:
				ShootPoint = BottomLeftShootPoint.global_position
			FacingDirection.BottomRight:
				ShootPoint = BottomRightShootPoint.global_position

		var direction = (ShootPoint - OriginPoint.global_position).normalized()
		projectile.Setup(self, direction, Data.Modes[Mode].Damage, target, TargetedEnemy)
		projectile.global_position = ShootPoint
		GameManager.GM.call_deferred("add_child", projectile)

	CanShoot = false

func OnLaserEntered(body: Node2D):
	if body is Enemy:
		EnemyEnteredLaser += 1
		if EnemyEnteredLaser >= 5:
			SelfDestruct()

func OnProjectileHit():
	if Mode == 0:
		SelfDestruct()
