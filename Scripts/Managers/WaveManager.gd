extends Node
class_name WaveManager

signal WaveEnded
signal WaveStarted
signal AllWavesFinished

enum FacingDirection {
	TopLeft,
	TopRight,
	BottomLeft,
	BottomRight
}

@export_group("Wave Data")
@export var SpawnLayer: TileMapLayer
@export var Waves: Array[WaveData]
@export var TimeBetweenBatches: float

@export_group("Wave Warning")
@export var IndicatorLayer: TileMapLayer
@export var IndicatorTime: float
@export var CountDownTime: float

var WaveQuery: Dictionary = {}

var CurrentWave: WaveData
var CurrentPath: Path2D
var CurrentWaveNumber: int

var CurrentBatches: Array[SpawnData] = []
var CurrentBatchesQueue: Dictionary = {}

var CurrentBatchIndex: int
var CurrentQueueIndex: int

var SpawnInterval: float
var SpawnTimer: float

static var WM: WaveManager
var SpawnFinished: bool = true
var WaveFinished: bool = true
var LevelFinished: bool
var CountDownFinished: bool
var Warned: bool

var Direction: FacingDirection
var LastDirection: FacingDirection
var WaveTimer: float
var BatchTimer: float
var CountDownTimer: float
var IndicatorTimer: float

func _ready():
	WM = self
	IndicatorTimer = IndicatorTime
	BatchTimer = TimeBetweenBatches
	CountDownTimer = CountDownTime
	CurrentWaveNumber = 1

	for wave in Waves:
		WaveQuery[wave.WaveNumber] = wave

	CurrentWave = WaveQuery[CurrentWaveNumber]
	WaveTimer = CurrentWave.TimeBetweenWaves

func _process(delta):
	if LevelFinished or Engine.is_editor_hint():
		return

	if not CountDownFinished and WaveFinished:
		CountDownTimer -= delta
		if CountDownTimer <= 0:
			CountDownFinished = true

	if CountDownFinished and WaveFinished:
		WaveTimer -= delta
		IndicatorTimer -= delta
		if WaveTimer <= 0:
			StartWave(CurrentWaveNumber)
		if not Warned and IndicatorTimer <= 0:
			WaveWarning(CurrentWaveNumber)

	if SpawnFinished and get_tree().get_nodes_in_group("Enemy").size() == 0 and not WaveFinished:
		WaveFinished = true
		CountDownTimer = CountDownTime
		IndicatorTimer = IndicatorTime
		Warned = false
		CountDownFinished = false
		emit_signal("WaveEnded")

		if CurrentWaveNumber < Waves.size():
			CurrentWaveNumber += 1
		else:
			LevelFinished = true
			emit_signal("AllWavesFinished")
			print("Level Finished")
			return

		WaveTimer = WaveQuery[CurrentWaveNumber].TimeBetweenWaves
		return

	if CurrentBatchesQueue.size() > 0:
		SpawnTimer -= delta
		if SpawnTimer <= 0:
			SpawnEnemy(CurrentBatches[CurrentQueueIndex].Enemy)
			CurrentQueueIndex += 1
			if CurrentBatches.size() == 0:
				return
			if CurrentQueueIndex >= CurrentBatches.size():
				CurrentQueueIndex = 0
			SpawnInterval = CurrentBatches[CurrentQueueIndex].SpawnInterval
			SpawnTimer = SpawnInterval

	elif not SpawnFinished:
		if CurrentBatchIndex >= CurrentWave.SpawnedEnemy.size():
			SpawnFinished = true
		if BatchTimer <= 0:
			BatchTimer = TimeBetweenBatches

	if CurrentBatchesQueue.size() == 0 and not SpawnFinished:
		BatchTimer -= delta
		if BatchTimer <= 0:
			NextBatch(CurrentBatchIndex)

func SpawnEnemy(enemy: PackedScene):
	var entity = enemy.instantiate() as Enemy
	var path = get_child(CurrentWaveNumber - 1).get_child(CurrentBatches[CurrentQueueIndex].PathID)
	entity.Setup(path)
	SpawnLayer.add_child(entity)

	CurrentBatchesQueue[CurrentBatches[CurrentQueueIndex]] -= 1
	if CurrentBatchesQueue[CurrentBatches[CurrentQueueIndex]] <= 0:
		CurrentBatchesQueue.erase(CurrentBatches[CurrentQueueIndex])
		CurrentBatches.remove_at(CurrentQueueIndex)

func WaveWarning(WaveNumber: int):
	CurrentWaveNumber = WaveNumber
	CurrentWave = WaveQuery[WaveNumber]
	Warned = true
	PlaceIndicator()

func StartWave(WaveNumber: int):
	SpawnFinished = false
	WaveFinished = false
	CurrentWaveNumber = WaveNumber
	CurrentWave = WaveQuery[WaveNumber]
	CurrentBatchIndex = 0
	CurrentQueueIndex = WaveNumber
	ClearIndication()
	NextBatch(CurrentBatchIndex)
	emit_signal("WaveStarted")

func NextBatch(BatchIndex: int):
	if not is_instance_valid(CurrentWave):
		return

	if BatchIndex >= CurrentWave.SpawnedEnemy.size():
		SpawnFinished = true
		return

	CurrentBatches.clear()
	CurrentBatchesQueue.clear()

	var first = CurrentWave.SpawnedEnemy[BatchIndex]
	CurrentBatches.append(first)
	CurrentBatchesQueue[first] = first.Count
	BatchIndex += 1

	while BatchIndex < CurrentWave.SpawnedEnemy.size() and CurrentWave.SpawnedEnemy[BatchIndex].Mode == SpawnData.SpawnMode.Parallel:
		var batch = CurrentWave.SpawnedEnemy[BatchIndex]
		CurrentBatches.append(batch)
		CurrentBatchesQueue[batch] = batch.Count
		BatchIndex += 1

	CurrentBatchIndex = BatchIndex
	CurrentQueueIndex = 0

func PlaceIndicator():
	IndicatorLayer.get_child(CurrentWaveNumber - 1).show()

func ClearIndication():
	IndicatorLayer.get_child(CurrentWaveNumber - 1).hide()
