using Godot;
using Godot.Collections;

public partial class BuildManager : Node
{
    [ExportGroup("Data")]
    [Export] private TurretData TurretInfo;
    
    [ExportGroup("References")]
    [Export] private TileMapLayer PreviewLayer;
    [Export] private TileMapLayer PlaceableLayer; //Layer to place the object one
    [Export] private TileMapLayer GroundLayer; //Layer for checking if placeable or not

    public bool IsBuilding = false;
    public Array<Vector2I> OccupiedTiles = new();
    
    public static BuildManager BM;

    private Vector2I PrevPos;
    private Vector2I GridPos;

    public override void _Ready()
    {
        BM = this;
    }

    public override void _Process(double delta)
    {
        if (IsBuilding)
            GridPos = PreviewLayer.LocalToMap(PreviewLayer.GetLocalMousePosition());

        if (IsPlacementValid())
            PreviewLayer.Modulate = new Color(1, 1, 1, 0.5f);
        else
            PreviewLayer.Modulate = new Color(1, 0, 0, 0.75f);
        
        if (Input.IsActionJustPressed("Build"))
            Build();

        if (Input.IsActionJustReleased("Interact") && IsBuilding && IsPlacementValid())
            Place();

        if (IsBuilding)
            PreviewPlacement();
    }

    public void SetTurret(TurretData TurretInfo)
    {
        this.TurretInfo = TurretInfo;
    }
    
    //Start building the turret
    public void Build()
    {
        IsBuilding = true;
    }

    
    //Place the turret
    public void Place()
    {
        PlaceableLayer.SetCell(GridPos, 2, Vector2I.Zero, TurretInfo.ID);
        
        //Add occupied spots
        for (int x = 0; x < TurretInfo.Size.X; x++)
        {
            for (int y = 0; y < TurretInfo.Size.Y; y++)
            {
                Vector2I pos = GridPos + new Vector2I(x, -y);
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
                Vector2I pos = GridPos + new Vector2I(x, -y);
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
}