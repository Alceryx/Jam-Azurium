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
    [Export] private TileMapLayer GroundLayer; //Layer for checking if placeable or not

    private Turret CurrentPreview;
    private Turret.FacingDirection PreviewRotation;
    
    public bool IsBuilding;
    public Array<Vector2I> OccupiedTiles = new();
    private Array<Vector2I> RecentlyOccupied = new();
    
    public static BuildManager BM;

    private Vector2I PrevPos;
    private Vector2I GridPos;

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
        
        if (!IsBuilding && Input.IsActionJustPressed("Build"))
            Build();

        if (Input.IsActionJustReleased("Interact") && IsBuilding && IsPlacementValid())
            Place();

        if (Input.IsActionJustPressed("Rotate") && TurretInfo.Modes[TurretMode].CanRotate && IsBuilding)
            Rotate();
            
        
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
                
                if (PreviewRotation == Turret.FacingDirection.TopRight)
                    pos = GridPos + new Vector2I(x, -y);
                else if (PreviewRotation == Turret.FacingDirection.TopLeft)
                    pos = GridPos + new Vector2I(-y, x);
                else if (PreviewRotation == Turret.FacingDirection.BottomRight)
                    pos = GridPos + new Vector2I(y, x);
                else
                    pos = GridPos + new Vector2I(x, y);
                
                OccupiedTiles.Add(pos);
                RecentlyOccupied.Add(pos);
            }
        }
    }
    
    //Check if placement is valid or not
    public bool IsPlacementValid()
    {
        for (int x = 0; x < TurretInfo.Size.X; x++)
        {
            for (int y = 0; y < TurretInfo.Size.Y; y++)
            {
                Vector2I pos = new Vector2I();
                if (PreviewRotation == Turret.FacingDirection.TopRight)
                    pos = GridPos + new Vector2I(x, -y);
                else if (PreviewRotation == Turret.FacingDirection.TopLeft)
                    pos = GridPos + new Vector2I(-y, x);
                else if (PreviewRotation == Turret.FacingDirection.BottomRight)
                    pos = GridPos + new Vector2I(y, x);
                else
                    pos = GridPos + new Vector2I(x, y);
                
                if (OccupiedTiles.Contains(pos))
                    return false;

                if (!IsInstanceValid(GroundLayer.GetCellTileData(pos)))
                    return false;

                if (!(bool)GroundLayer.GetCellTileData(pos).GetCustomData("Placeable"))
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
    }
}