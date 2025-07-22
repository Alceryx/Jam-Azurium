using Godot;
using System;

/*
 * Use to display a ghost version of the turret before placing
 */
public partial class GhostTurret : Sprite2D
{
    private Vector2 TurretSize;
    private TileMapLayer TileMap;
    private bool PlacementValid;

    public void Setup(Vector2 TurretSize, TileMapLayer TileMap, Texture Texture)
    {
        this.TurretSize = TurretSize;
        this.TileMap = TileMap;
        this.Texture = (Texture2D)Texture;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (!BuildManager.BM.IsBuilding) return;
        
        GlobalPosition = SnapToGrid(GetGlobalMousePosition());

        PlacementValid = BuildManager.BM.IsPlacementValid(GlobalPosition);

        if (PlacementValid)
            Modulate = new Color(1, 1, 1, .5f);
        else
            Modulate = new Color(1, 0, 0, .75f);
        
        if (Input.IsActionJustReleased("Interact") && PlacementValid)
        {
            BuildManager.BM.Place(GlobalPosition);
        }
    }


    //Function to snap world position to grid
    private Vector2 SnapToGrid(Vector2 WorldPosition)
    {
        Vector2 LocalPos = TileMap.ToLocal(WorldPosition); //Convert global position to local layer map position
        Vector2I GridPos = TileMap.LocalToMap(LocalPos); //Snap local position to grid map
        Vector2 SnappedWorldPos = TileMap.MapToLocal(GridPos); //Convert grid map back to local position (centered)
        SnappedWorldPos += new Vector2(TileMap.TileSet.TileSize.X * 0.2f, TileMap.TileSet.TileSize.Y * -0.2f); //Apply vector offset
        
        return TileMap.ToGlobal(SnappedWorldPos); //Convert back to global position
    }
}
