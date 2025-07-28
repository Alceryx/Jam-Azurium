using Godot;
using Godot.Collections;
using System;

public partial class WaveManager : Node
{
    [Export] public Array<WaveData> Waves;
    [Export] public Path2D WavePath;
    private Dictionary<int, WaveData> WaveQuery = new();
    
    public WaveData CurrentWave;
    public int CurrentWaveNumber;
    public Array<SpawnData> CurrentBatches = new();
    public Dictionary<SpawnData, int> CurrentBatchesQueue = new();
    public int CurrentBatchIndex; 
    public int CurrentQueueIndex;

    private float SpawnInterval;
    private float SpawnTimer;
    
    public static WaveManager WM;
    public bool SpawnFinished = false;
    public bool WaveFinished = false;

    [Signal]
    public delegate void WaveEndedEventHandler(int WaveNumber);
    
    [Signal]
    public delegate void WaveStartedEventHandler();
    
    public override void _Ready()
    {
        WM = this;

        foreach (WaveData wave in  Waves)
        {
            WaveQuery.Add(wave.WaveNumber, wave);
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("NextWave"))
            StartWave(1);

        if (SpawnFinished && GetTree().GetNodesInGroup("Enemy").Count == 0 && !WaveFinished)
        {
            WaveFinished = true;
            EmitSignalWaveEnded(CurrentWaveNumber);
            return;
        }
        
        if (CurrentBatchesQueue.Count > 0)
        {
            SpawnTimer -= (float)delta;
            if (SpawnTimer <= 0)
            {
                SpawnEnemy(CurrentBatches[CurrentQueueIndex].Enemy);
                CurrentQueueIndex++;

                if (CurrentBatches.Count == 0)
                    return;
                
                if (CurrentQueueIndex >= CurrentBatches.Count)
                    CurrentQueueIndex = 0;
                
                SpawnInterval = CurrentBatches[CurrentQueueIndex].SpawnInterval;
                SpawnTimer = SpawnInterval;
            }
        }
        else if (!SpawnFinished)
            NextBatch(CurrentBatchIndex);
    }

    private void SpawnEnemy(PackedScene Enemy)
    {
        Enemy enemy = Enemy.Instantiate() as Enemy;
        enemy.Setup(WavePath);
        GameManager.GM.AddChild(enemy);

        CurrentBatchesQueue[CurrentBatches[CurrentQueueIndex]] -= 1;
        if (CurrentBatchesQueue[CurrentBatches[CurrentQueueIndex]] <= 0)
        {
            CurrentBatchesQueue.Remove(CurrentBatches[CurrentQueueIndex]);
            CurrentBatches.RemoveAt(CurrentQueueIndex);
        }

    }
    
    public void StartWave(int WaveNumber)
    {
        CurrentWave = WaveQuery[WaveNumber];
        CurrentBatchIndex = 0;
        CurrentQueueIndex = WaveNumber;
        NextBatch(CurrentBatchIndex);
        EmitSignalWaveStarted();
    }

    public void NextBatch(int BatchIndex)
    {
        if (!IsInstanceValid(CurrentWave))
            return;
        
        if (BatchIndex >= CurrentWave.SpawnedEnemy.Count)
        {
            SpawnFinished = true;
            return;
        }
        
        CurrentBatches.Clear();
        CurrentBatchesQueue.Clear();
        
        CurrentBatches.Add(CurrentWave.SpawnedEnemy[BatchIndex]);
        CurrentBatchesQueue.Add(CurrentWave.SpawnedEnemy[BatchIndex], CurrentWave.SpawnedEnemy[BatchIndex].Count);

        BatchIndex++;
        while (BatchIndex < CurrentWave.SpawnedEnemy.Count && CurrentWave.SpawnedEnemy[BatchIndex].Mode == SpawnData.SpawnMode.Parallel)
        {
            CurrentBatches.Add(CurrentWave.SpawnedEnemy[BatchIndex]);
            CurrentBatchesQueue.Add(CurrentWave.SpawnedEnemy[BatchIndex], CurrentWave.SpawnedEnemy[BatchIndex].Count);
            BatchIndex++;
        }
        
        CurrentBatchIndex = BatchIndex;
        CurrentQueueIndex = 0;
    }
}
