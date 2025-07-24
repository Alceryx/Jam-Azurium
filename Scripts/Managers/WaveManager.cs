using Godot;
using Godot.Collections;
using System;

public partial class WaveManager : Node
{
    [Export] public Array<WaveData> Waves;
    [Export] public Path2D WavePath;
    private Dictionary<int, WaveData> WaveQuery = new();

    public WaveData CurrentWave;
    public Array<SpawnData> CurrentBatches = new();
    public Dictionary<SpawnData, int> CurrentBatchesQueue = new();
    public int CurrentBatchIndex; 
    public int CurrentQueueIndex;

    private float SpawnInterval;
    private float SpawnTimer;
    
    public static WaveManager WM;
    public bool WaveFinished = false;
    
    public override void _Ready()
    {
        WM = this;

        foreach (WaveData wave in  Waves)
        {
            WaveQuery.Add(wave.WaveNumber, wave);
        }
        
        StartWave(1);
    }

    public override void _Process(double delta)
    {
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
        else if (!WaveFinished)
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
        NextBatch(CurrentBatchIndex);
    }

    public void NextBatch(int BatchIndex)
    {
        if (BatchIndex >= CurrentWave.SpawnedEnemy.Count)
        {
            WaveFinished = true;
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
