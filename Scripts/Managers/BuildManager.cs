using Godot;
using Godot.Collections;

public partial class BuildManager : Node
{
    [ExportGroup("Data")]
    [Export] private TurretData TurretInfo;
    [Export] private int TurretMode = 1;
    
    [ExportGroup("References")]
    [Export] private TileMapLayer PreviewLayer;
    [Export] public TileMapLayer PlaceableLayer; //Layer to place the object one
    [Export] private Array<TileMapLayer> GroundLayer; //Layer for checking if placeable or not
    [Export] private Array<TileMapLayer> UnplaceableLayer; //Layer that is unplaceable on.

    private Turret CurrentPreview;
    private Turret.FacingDirection PreviewRotation;
    
    public bool IsBuilding;
    public Array<Vector2I> OccupiedTiles = new();
    private Array<Vector2I> RecentlyOccupied = new();
    
    public static BuildManager BM;

    private Vector2I PrevPos;
    private Vector2I GridPos;

    [Signal]
    public delegate void PlacedTurretEventHandler(TurretData data, int mode);

    public override void _Ready()
    {
        BM = this;
        PreviewLayer.ChildEnteredTree += (Node node) => UpdatePreviewData(node);
        PlaceableLayer.ChildEnteredTree += (Node node) => UpdatePlacementData(node);
    }

    public override void _Process(double delta)
    {
        if (IsBuilding)
            GridPos = PreviewLayer.LocalToMap(PreviewLayer.GetLocalMousePosition());

        if (IsPlacementValid())
            PreviewLayer.Modulate = new Color(1, 1, 1, 0.5f);
        else
            PreviewLayer.Modulate = new Color(1, 0, 0, 0.75f);

        if (Input.IsActionJustReleased("Interact") && IsBuilding && IsPlacementValid())
            Place();

        if (Input.IsActionJustPressed("Rotate") && TurretInfo.Modes[TurretMode].CanRotate && IsBuilding)
            Rotate();
        
        if (Input.IsActionJustPressed("Escape") && IsBuilding)
            CancelPlacement();
            
        
        if (IsBuilding)
            PreviewPlacement();
    }

    public void SetTurret(TurretData TurretInfo, int Mode)
    {
        this.TurretInfo = TurretInfo;
        this.TurretMode = Mode;
    }
    
    //Start building the turret
    public void Build()
    {
        IsBuilding = true;
        PreviewRotation = 0;
    }

    public void Destroy(Turret turret)
    {
        foreach (Vector2I pos in turret.OccupiedPositions)
        {
            OccupiedTiles.Remove(pos + Vector2I.One);
        }
        BM.PlaceableLayer.EraseCell(turret.OccupiedPositions[0]);
        
        
    }

    public void CancelPlacement()
    {
        PreviewLayer.EraseCell(GridPos);
        IsBuilding = false;
    }
    
    public void Store(Turret turret)
    {
        Destroy(turret);
        
        if (HotbarManager.HM.TurretsSlot.ContainsKey(turret.Data))
        {
            if (HotbarManager.HM.TurretsSlot[turret.Data].ContainsKey(turret.Mode))
                HotbarManager.HM.UpdateSlot(turret.Data, turret.Mode);
            else
                HotbarManager.HM.AddSlot(turret.Data, turret.Mode);
        }
        else
            HotbarManager.HM.AddSlot(turret.Data, turret.Mode);
    }

    public void Move(Turret turret)
    {
        Destroy(turret);
        SetTurret(turret.Data, turret.Mode);
        Build();
    }
 
    //Rotate the turret
    public void Rotate()
    {
        PreviewRotation += 1;
        if ((int)PreviewRotation > 3)
            PreviewRotation = 0;
        UpdatePreviewData(CurrentPreview);
    }
    
    //Place the turret
    public void Place()
    {
        Vector2I RealPos = GridPos + Vector2I.One;
        IsBuilding = false;
        PreviewLayer.EraseCell(GridPos);
        PlaceableLayer.SetCell(GridPos, 2, Vector2I.Zero, TurretInfo.ID);
        RecentlyOccupied.Clear();
        //Add occupied spots
        
        for (int x = 0; x < TurretInfo.Size.X; x++)
        {
            for (int y = 0; y < TurretInfo.Size.Y; y++)
            {
                Vector2I pos = new Vector2I();
                Vector2I display = new Vector2I();

                if (PreviewRotation == Turret.FacingDirection.TopRight)
                {
                    pos = RealPos + new Vector2I(x, -y);
                    display = GridPos + new Vector2I(x, -y);
                }
                else if (PreviewRotation == Turret.FacingDirection.TopLeft)
                {
                    pos = RealPos + new Vector2I(-y, x);
                    display = GridPos + new Vector2I(-y, x);
                }
                else if (PreviewRotation == Turret.FacingDirection.BottomRight)
                {
                    pos = RealPos + new Vector2I(y, x);
                    display = GridPos + new Vector2I(y, x);
                }
                else
                { 
                    pos = RealPos + new Vector2I(x, y);
                    display = GridPos + new Vector2I(x, y);
                }
                
                OccupiedTiles.Add(pos);
                RecentlyOccupied.Add(display);
            }
        }
    }
    
    //Check if placement is valid or not
    public bool IsPlacementValid()
    {
        Vector2I RealPos = GridPos + Vector2I.One;
        
        for (int x = 0; x < TurretInfo.Size.X; x++)
        {
            for (int y = 0; y < TurretInfo.Size.Y; y++)
            {
                Vector2I pos = new Vector2I();
                if (PreviewRotation == Turret.FacingDirection.TopRight)
                    pos = RealPos + new Vector2I(x, -y);
                else if (PreviewRotation == Turret.FacingDirection.TopLeft)
                    pos = RealPos + new Vector2I(-y, x);
                else if (PreviewRotation == Turret.FacingDirection.BottomRight)
                    pos = RealPos + new Vector2I(y, x);
                else
                    pos = RealPos + new Vector2I(x, y);
                
                if (OccupiedTiles.Contains(pos))
                    return false;

                foreach (TileMapLayer layer in UnplaceableLayer)
                {
                    if (IsInstanceValid(layer.GetCellTileData(pos)))
                        return false;
                }
                
                foreach (TileMapLayer layer in GroundLayer)
                {
                    if (IsInstanceValid(layer.GetCellTileData(pos)) && (bool)layer.GetCellTileData(pos).GetCustomData("Placeable"))
                        return true;
                }

                return false;

            }
        }
        
        return true;
    }
    
    
    private void PreviewPlacement()
    {
        if (PrevPos != GridPos)
            PreviewLayer.EraseCell(PrevPos);

        PreviewLayer.SetCell(GridPos, 2, Vector2I.Zero, TurretInfo.ID);
        PrevPos = GridPos;
    }

    private void UpdatePreviewData(Node node)
    {
        if (node is Turret)
        {
            CurrentPreview = (Turret)node;

            if (!IsInstanceValid(CurrentPreview.Sprite))
                return;
            CurrentPreview.SetUp(TurretInfo, TurretMode, PreviewRotation, Preview:true);
            CurrentPreview.ShowDetectionPreview();
        }
    }

    private void UpdatePlacementData(Node node)
    {
        if (node is not Turret)
            return;
        
        Turret turret = (Turret)node;
        
        Array<Vector2I> temp = new Array<Vector2I>();
        temp.AddRange(RecentlyOccupied);
        
        turret.SetUp(TurretInfo, TurretMode, PreviewRotation, temp); 
        turret.HideDetectionPreview();
        
        EmitSignalPlacedTurret(TurretInfo, TurretMode);
    }
}