using Godot;
using Godot.Collections;

public partial class BuildManager : Node
{
    [ExportGroup("Data")]
    [Export] private TurretData TurretInfo;
    
    [ExportGroup("References")]
    [Export] private TileMapLayer TileMap;
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

    public void Build()
    {
        IsBuilding = true;
        GhostTurret.Setup(TurretInfo.Size, TileMap, TurretInfo.Icon);
        GhostTurret.Show();
    }

    public void Place(Vector2 position)
    {
        IsBuilding = false;
        GhostTurret.Hide();
        Turret turret = TurretInfo.Turret.Instantiate() as Turret;
        turret.GlobalPosition = position;
        GameManager.GM.AddChild(turret);

        
        //---------
        Vector2 LocalPos = TileMap.ToLocal(position);
        Vector2I GridPos = TileMap.LocalToMap(LocalPos);

        Vector2I New = GridPos + new Vector2I(1, -3);
        
        Vector2 SnappedWorldPos = TileMap.MapToLocal(New);
        SnappedWorldPos += new Vector2(TileMap.TileSet.TileSize.X / 2, TileMap.TileSet.TileSize.Y / 2) / TurretInfo.Size; 
        
        Turret turret2 = TurretInfo.Turret.Instantiate() as Turret;
        turret2.GlobalPosition =  TileMap.ToGlobal(SnappedWorldPos);
        GameManager.GM.AddChild(turret2);
        
    }
    
    public bool IsPlacementValid(Vector2 WorldPosition)
    {
        return true;
    }
}
