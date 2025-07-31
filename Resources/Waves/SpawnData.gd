# SpawnData.gd
@tool
extends Resource
class_name SpawnData

enum SpawnMode {
	Sequential,
	Parallel
}

@export var Enemy: PackedScene
@export var Count: int
@export var SpawnInterval: float    # The Interval between each enemy spawned
@export var PathID: int
@export var Mode: SpawnMode
