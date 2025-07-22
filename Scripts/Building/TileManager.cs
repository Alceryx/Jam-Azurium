using Godot;
using System;

public partial class TileManager : TileMapLayer
{
    private Vector2I Start;
    private Vector2I End;

    private Vector2I MousePos;

    public override void _PhysicsProcess(double delta)
    {
        
    }

    public void BuildRect(Vector2I Start, Vector2I End)
    {
        for (int x = Start.X; x <= End.X; x++)
        {
            for (int y = Start.Y; y <= End.Y; y++)
            {
                GetChild<TileMapLayer>(0).SetCell(new Vector2I(x, y), 0, new Vector2I(0,0));
            }
        }
    }
}
