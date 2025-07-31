extends Node
class_name Vec2Extensions

static func get_isometric_angle_to(from: Vector2, to: Vector2) -> float:
	var dir: Vector2 = to - from
	var iso_vector: Vector2 = cartesian_to_isometric(dir)

	var angle: float = rad_to_deg(iso_vector.angle())
	if angle < 0:
		angle += 360

	return angle

static func cartesian_to_isometric(vector: Vector2) -> Vector2:
	return Vector2(vector.x - vector.y, (vector.x + vector.y) / 2.0)
