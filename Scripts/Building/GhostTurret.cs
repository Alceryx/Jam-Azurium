using Godot;
using System;

/*
 * Use to display a ghost version of the turret before placing
 */
public partial class GhostTurret : Sprite2D
{
    private Vector2 TurretSize;
    private TileMapLayer TileMap;

    public void Setup(Vector2 TurretSize, TileMapLayer TileMap, Texture Texture)
    {
        this.TurretSize = TurretSize;
        this.TileMap = TileMap;
        this.Texture = (Texture2D)Texture;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (!BuildManager.BM.IsBuilding)
            return;
        
        GlobalPosition = SnapToGrid(GetGlobalMousePosition());
        
        GD.Print(BuildManager.BM.IsPlacementValid(GlobalPosition));
        
        if (Input.IsActionJustReleased("Place"))
        {
            BuildManager.BM.Place(GlobalPosition);
        }
    }


    //Function to snap world position to grid
    private Vector2 SnapToGrid(Vector2 WorldPosition)
    {
        Vector2 LocalPos = TileMap.ToLocal(WorldPosition);
        Vector2I GridPos = TileMap.LocalToMap(LocalPos);
        Vector2 SnappedWorldPos = TileMap.MapToLocal(GridPos);
        SnappedWorldPos += new Vector2(TileMap.TileSet.TileSize.X / 2, TileMap.TileSet.TileSize.Y / 2) / TurretSize; 
        
        return TileMap.ToGlobal(SnappedWorldPos);
    }
}
