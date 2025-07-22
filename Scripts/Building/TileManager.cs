using Godot;
using System;

public partial class TileManager : TileMapLayer
{
    [Export] private TileMapLayer PreviewTile;
    [Export] private TileMapLayer PlaceableTile;
    private Vector2I PrevPos;

    public override void _PhysicsProcess(double delta)
    {
        
        Vector2I GridPos = PreviewTile.LocalToMap(PreviewTile.GetLocalMousePosition());
        if (PrevPos != GridPos)
            PreviewTile.EraseCell(PrevPos);
        PreviewTile.SetCell(GridPos, 3, Vector2I.Zero);
        PrevPos = GridPos;

        if (Input.IsActionJustPressed("Interact"))
        {
            PlaceableTile.SetCell(GridPos, 3, Vector2I.Zero);
        }
    }
}
