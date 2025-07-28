using Godot;
using Godot.Collections;
using System;

public partial class WaveManager : Node
{
    private enum FacingDirection
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    
    [ExportGroup("Wave Data")]
    [Export] public Array<WaveData> Waves;
    [Export] private float TimeBetweenWaves;
    [Export] private float TimeBetweenBatches;
    [ExportGroup("Wave Warning")]
    [Export] private TileMapLayer Indicator;
    [Export] private float IndicatorTime;
    private Dictionary<int, WaveData> WaveQuery = new();
    
    public WaveData CurrentWave;
    public Path2D CurrentPath;
    public int CurrentWaveNumber;
    public Array<SpawnData> CurrentBatches = new();
    public Dictionary<SpawnData, int> CurrentBatchesQueue = new();
    public int CurrentBatchIndex; 
    public int CurrentQueueIndex;

    private float SpawnInterval;
    private float SpawnTimer;
    
    public static WaveManager WM;
    public bool SpawnFinished = true;
    public bool WaveFinished = true;
    private bool Warned;
    
    private FacingDirection Direction;
    private FacingDirection LastDirection;
    private float WaveTimer;
    private float BatchTimer;
    private float IndicatorTimer;

    [Signal]
    public delegate void WaveEndedEventHandler();
    
    [Signal]
    public delegate void WaveStartedEventHandler();
    
    public override void _Ready()
    {
        WM = this;
        WaveTimer = TimeBetweenWaves;
        IndicatorTimer =  IndicatorTime;
        BatchTimer = TimeBetweenBatches;
        CurrentWaveNumber = 1;
        CurrentPath = GetChild<Path2D>(CurrentWaveNumber - 1);
        
        foreach (WaveData wave in Waves)
        {
            WaveQuery.Add(wave.WaveNumber, wave);
        }
        
        CurrentWave = WaveQuery[CurrentWaveNumber];
    }

    public override void _Process(double delta)
    {
        if (WaveFinished)
        {
            WaveTimer -= (float)delta;
            IndicatorTimer -= (float)delta;
            if (WaveTimer <= 0)
                StartWave(CurrentWaveNumber);

            if (!Warned && IndicatorTimer <= 0)
                WaveWarning(CurrentWaveNumber);
        }
        
        if (SpawnFinished && GetTree().GetNodesInGroup("Enemy").Count == 0 && !WaveFinished)
        {
            WaveFinished = true;
            WaveTimer = TimeBetweenWaves;
            IndicatorTimer = IndicatorTime;
            Warned = false;
            EmitSignalWaveEnded();
            
            if (CurrentWaveNumber <= Waves.Count - 1)
                CurrentWaveNumber++;
            else
                CurrentWaveNumber = 1;
            
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
        {
            if (CurrentBatchIndex >= CurrentWave.SpawnedEnemy.Count)
                SpawnFinished = true;
            
            if (BatchTimer <= 0)
                BatchTimer = TimeBetweenBatches;
        }
            
        

        if (CurrentBatchesQueue.Count <= 0 && !SpawnFinished)
        {
            BatchTimer -= (float)delta;
            if (BatchTimer <= 0)
                NextBatch(CurrentBatchIndex);
        }
    }

    private void SpawnEnemy(PackedScene Enemy)
    {
        Enemy enemy = Enemy.Instantiate() as Enemy;
        enemy.Setup(CurrentPath);
        GameManager.GM.AddChild(enemy);

        CurrentBatchesQueue[CurrentBatches[CurrentQueueIndex]] -= 1;
        if (CurrentBatchesQueue[CurrentBatches[CurrentQueueIndex]] <= 0)
        {
            CurrentBatchesQueue.Remove(CurrentBatches[CurrentQueueIndex]);
            CurrentBatches.RemoveAt(CurrentQueueIndex);
        }

    }

    private void WaveWarning(int WaveNumber)
    {
        CurrentWaveNumber = WaveNumber;
        CurrentWave = WaveQuery[WaveNumber];
        CurrentPath = GetChild<Path2D>(WaveNumber - 1);
        Warned = true;
        PlaceIndicator();
    }
    
    public void StartWave(int WaveNumber)
    {
        SpawnFinished = false;
        WaveFinished = false;
        CurrentWaveNumber = WaveNumber;
        CurrentWave = WaveQuery[WaveNumber];
        CurrentBatchIndex = 0;
        CurrentQueueIndex = WaveNumber;
        ClearIndication();
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

    private void PlaceIndicator()
    {
        Curve2D curve = CurrentPath.GetCurve();
        Vector2[] points = curve.GetBakedPoints();
        LastDirection = FacingDirection.TopLeft;
        
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2I GridPos = Indicator.LocalToMap(Indicator.ToLocal(CurrentPath.ToGlobal(points[i])));
            
            float angle = points[i].GetIsometricAngleTo(points[i + 1]);
        
            if (angle >= 225 && angle < 315)
                Direction = FacingDirection.TopLeft;
            else if (angle >= 315 || angle < 45)
                Direction = FacingDirection.TopRight;
            else if (angle >= 135 && angle < 225)
                Direction = FacingDirection.BottomLeft;
            else if (angle >= 45 && angle < 135)
                Direction = FacingDirection.BottomRight;

            
            if (Direction == FacingDirection.TopLeft || Direction == FacingDirection.BottomRight)
                Indicator.SetCell(GridPos, 3, new Vector2I(4, 0));
            else
                Indicator.SetCell(GridPos, 3, new Vector2I(5, 0));
            
            if (LastDirection != null)
            {
                if ((LastDirection == FacingDirection.TopLeft && Direction == FacingDirection.TopRight) || 
                    (Direction == FacingDirection.BottomRight && LastDirection == FacingDirection.BottomLeft))
                    Indicator.SetCell(GridPos, 3, new Vector2I(1, 0));
                else if ((LastDirection == FacingDirection.TopRight && Direction == FacingDirection.BottomRight) ||
                         (Direction == FacingDirection.BottomLeft && LastDirection == FacingDirection.TopLeft))
                    Indicator.SetCell(GridPos, 3, new Vector2I(3, 0));
                else if ((LastDirection == FacingDirection.BottomRight && Direction == FacingDirection.BottomLeft) ||
                         (Direction == FacingDirection.TopLeft && LastDirection == FacingDirection.TopRight))
                    Indicator.SetCell(GridPos, 3, new Vector2I(2, 0));
                else if ((LastDirection == FacingDirection.BottomLeft && Direction == FacingDirection.TopLeft) || 
                        (Direction == FacingDirection.TopRight && LastDirection == FacingDirection.BottomRight))
                    Indicator.SetCell(GridPos, 3, new Vector2I(0, 0));
            }
            
            LastDirection = Direction;
        }
    }

    private void ClearIndication()
    {
        foreach (Vector2I cell in Indicator.GetUsedCells())
        {
            Indicator.EraseCell(cell);
        }
    }
}
