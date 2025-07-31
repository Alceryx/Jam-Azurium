extends Line2D

@export var Length: int
var CurrentPosition: Vector2

func _process(delta: float) -> void:
	CurrentPosition = get_parent().global_position
	add_point(CurrentPosition)
	if points.size() > Length:
		remove_point(0)
