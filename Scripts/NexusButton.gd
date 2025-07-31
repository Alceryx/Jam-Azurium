extends StaticBody2D
class_name NexusButton

@export var MaxCPS: int
@export var HoldThreshold: float
@export var SpriteState: AnimationPlayer
@export var Sprite: Sprite2D
@export var Particle: PackedScene
@export var AudioPlayer: AudioStreamPlayer2D

var CanClick: bool
var Releasing: bool

var ClickDelay: float
var ClickTimer: float
var HoldTimer: float

var RiseDelay: float = 0.3
var RiseTimer: float

func _ready():
	SpriteState.play("Neutral")
	ClickDelay = 1.0 / MaxCPS
	call_deferred("SetInstance")

func _process(delta: float) -> void:
	if ShopManager.SM.visible:
		return
	
	if WaveManager.WM.WaveFinished and WaveManager.WM.CountDownFinished:
		if Releasing and RiseTimer > 0:
			SpriteState.play("Mid")
			RiseTimer -= delta
		elif not Releasing:
			RiseTimer = RiseDelay

		if RiseTimer <= 0:
			SpriteState.play("Neutral")
	else:
		SpriteState.play("Neutral")

	if ClickTimer > 0:
		ClickTimer -= delta

	if HoldTimer >= HoldThreshold and WaveManager.WM.WaveFinished and WaveManager.WM.CountDownFinished:
		ShopManager.SM.show()
		HoldTimer = 0

	if not BuildManager.BM.IsBuilding and CanClick and ClickTimer <= 0 and WaveManager.WM.WaveFinished and WaveManager.WM.CountDownFinished:
		SpriteState.play("Mid")

		if Input.is_action_just_pressed("Interact"):
			ClickTimer = ClickDelay
			GameManager.GM.Currency += GameManager.GM.Efficiency
			AudioPlayer.play()
			SpawnCloud()

		if Input.is_action_pressed("Interact"):
			SpriteState.play("Clicked")
			HoldTimer += delta

		if Input.is_action_just_released("Interact"):
			HoldTimer = 0

func TakeDamage(damage: float) -> void:
	GameManager.GM.ButtonHP -= damage

func SpawnCloud() -> void:
	var SmokeCloud: GPUParticles2D = Particle.instantiate() as GPUParticles2D
	add_child(SmokeCloud)
	move_child(SmokeCloud, 0)
	SmokeCloud.position = Vector2(0, 64)
	SmokeCloud.emitting = true

func OnMouseEntered() -> void:
	Releasing = false
	CanClick = true

func OnMouseExited() -> void:
	Releasing = true
	CanClick = false

func SetInstance() -> void:
	GameManager.GM.NexusButton = self
