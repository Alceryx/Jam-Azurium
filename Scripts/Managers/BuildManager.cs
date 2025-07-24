using Godot;
using Godot.Collections;

public partial class BuildManager : Node
{
    [ExportGroup("Data")]
    [Export] private TurretData TurretInfo;
    [Export] private int TurretMode = 1;
    
    [ExportGroup("References")]
    [Export] private TileMapLayer PreviewLayer;
    [Export] private TileMapLayer PlaceableLayer; //Layer to place the object one
    [Export] private TileMapLayer GroundLayer; //Layer for checking if placeable or not

    private Turret CurrentPreview;
    private Turret.FacingDirection PreviewRotation;
    
    public bool IsBuilding = false;
    public Array<Vector2I> OccupiedTiles = new();
    
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
            
            if (TurretInfo.Modes[TurretMode].CanRotate)
            {
                switch (PreviewRotation)
                {
                    case Turret.FacingDirection.TopLeft:
                        CurrentPreview.Sprite.Texture = TurretInfo.Modes[TurretMode].TopLeft;
                        break;
                    case Turret.FacingDirection.TopRight:
                        CurrentPreview.Sprite.Texture = TurretInfo.Modes[TurretMode].TopRight;
                        break;
                    case Turret.FacingDirection.BottomLeft:
                        CurrentPreview.Sprite.Texture = TurretInfo.Modes[TurretMode].BottomLeft;
                        break;
                    case Turret.FacingDirection.BottomRight:
                        CurrentPreview.Sprite.Texture = TurretInfo.Modes[TurretMode].BottomRight;
                        break;
                }
            }
            else
                CurrentPreview.Sprite.Texture = TurretInfo.Modes[TurretMode].Icon;
        }
    }

    private void UpdatePlacementData(Node node)
    {
        if (node is Turret)
        {
            Turret turret = (Turret)node;
            turret.SetUp(TurretInfo, TurretMode, PreviewRotation); 
        }
    }
}