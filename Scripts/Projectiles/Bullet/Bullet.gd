extends Projectile
class_name Bullet

@export var Speed: float

func _physics_process(delta: float) -> void:
	velocity = Direction * Speed
	look_at(Direction * 10000)
	move_and_slide()
