# TurretMode.gd
@tool
extends Resource
class_name TurretMode

@export_group("Basic Info")
@export var Icon: Texture2D                       # Icon (Preview icon)
@export var Price: int
@export var CanRotate: bool                       # Can Rotate on placing
@export_multiline var Description: String = ""              # Multiline text

@export_group("Stats")
@export var Projectile: PackedScene
@export var Damage: float
@export_range(0, 100, 1, "or_greater", "suffix:tiles") var ShootRange: float
