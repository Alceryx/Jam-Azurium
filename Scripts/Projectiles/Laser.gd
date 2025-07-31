extends Projectile

@export var CastSpeed: float
@export var RayCast: RayCast2D
@export var Line: Line2D

var Ray: SeparationRayShape2D
var Collider: CollisionShape2D

func _ready() -> void:
	Collider = Hitbox.get_child(0) as CollisionShape2D
	Ray = SeparationRayShape2D.new()
	Collider.shape = Ray

func _physics_process(delta: float) -> void:
	if not is_instance_valid(turret):
		queue_free()
		return

	RayCast.target_position = RayCast.target_position.move_toward(Direction * 10000, CastSpeed * delta)
	Ray.length = global_position.distance_to(to_global(RayCast.target_position))
	Collider.rotation = (to_global(RayCast.target_position) - global_position).normalized().angle() - PI / 2

	var EndPoint = RayCast.target_position
	RayCast.force_raycast_update()
	if RayCast.is_colliding():
		EndPoint = to_local(RayCast.get_collision_point())

	Line.set_point_position(1, EndPoint)
