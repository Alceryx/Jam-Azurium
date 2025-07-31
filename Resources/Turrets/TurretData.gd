# TurretData.gd
@tool
extends Resource
class_name TurretData

@export var ID: int               # ID of the turret for spawning
@export var Name: String          # Name of the turret
@export var Size: Vector2         # Size (in grid cells)
@export var Modes: Array[TurretMode]
