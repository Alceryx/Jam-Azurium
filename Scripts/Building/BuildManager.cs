using Godot;
using Godot.Collections;

public partial class BuildManager : Node
{
    [ExportGroup("Data")]
    [Export] private TurretData TurretInfo;
    
    [ExportGroup("References")]
    [Export] private TileMapLayer TileMap;
    [Export] private TileMapLayer PlaceableLayer;
    [Export] private GhostTurret GhostTurret;

    public bool IsBuilding = false;
    public Array<Vector2I> OccupiedTiles = new Array<Vector2I>();
    
    public static BuildManager BM;

    public override void _Ready()
    {
        BM = this;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Build"))
        {
            Build();
        }
    }

    //Start building the turret
    public void Build()
    {
        IsBuilding = true;
        GhostTurret.Setup(TurretInfo.Size, TileMap, TurretInfo.Icon);
        GhostTurret.Show();
    }

    
    //Place the turret
    public void Place(Vector2 position)
    {
        //Spawn turret
        IsBuilding = false;
        GhostTurret.Hide();
        Turret turret = TurretInfo.Turret.Instantiate() as Turret;
        turret.GlobalPosition = position;
        GameManager.GM.AddChild(turret);
        
        //Add occupied spots
        Vector2I GridPos = TileMap.LocalToMap(TileMap.ToLocal(position));
        
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
    public bool IsPlacementValid(Vector2 WorldPosition)
    {
        Vector2I GridPos = TileMap.LocalToMap(TileMap.ToLocal(WorldPosition));
        
        for (int x = 0; x < TurretInfo.Size.X; x++)
        {
            for (int y = 0; y < TurretInfo.Size.Y; y++)
            {
                Vector2I pos = GridPos + new Vector2I(x, -y);
                if (OccupiedTiles.Contains(pos))
                    return false;

                if (!IsInstanceValid(PlaceableLayer.GetCellTileData(pos))) 
                    return false;
                if (!(bool)PlaceableLayer.GetCellTileData(pos).GetCustomData("Placeable"))
                    return false;
            }
        }
        
        return true;
    }
}
