using Godot;
using System;

public partial class BulletTrail : Line2D
{
    [Export] private int Length;
    
    public override void _Process(double delta)
    {
        Vector2 CurrentPosition = GetParent<Node2D>().GlobalPosition;
        
        AddPoint(CurrentPosition);
        if (Points.Length > Length)
        {
            RemovePoint(0);
        }
    }
}
