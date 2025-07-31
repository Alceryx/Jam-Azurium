extends Node
class_name SceneManager

static var SM: SceneManager

var current_scene: Node

func _ready():
	SM = self
	current_scene = get_tree().root.get_child(get_tree().root.get_child_count() - 1)

func switch_scene(scene_path: String) -> void:
	call_deferred("deferred_switch", scene_path)

func preserve_on_load(node: Node) -> void:
	node.reparent(self)

func deferred_switch(scene_path: String) -> void:
	if current_scene:
		current_scene.queue_free()
	
	var new_scene = load(scene_path) as PackedScene
	current_scene = new_scene.instantiate()
	get_tree().root.add_child(current_scene)
	get_tree().current_scene = current_scene
