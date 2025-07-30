using Godot;
using System;

public partial class BulletTrail : Line2D
{
    Vector2 PreviousPosition = Vector2.Zero;
    private float Radius = 0;

    public override void _Ready()
    {
        //Texture2D texture = ((Sprite2D)GetParent()).Texture;
        Width = 500; //texture.GetSize().X * .5f;
        PreviousPosition = ((Sprite2D)GetParent()).GlobalPosition;
    }

    public override void _Process(double delta)
    {
        Vector2 CurrentPosition = ((Sprite2D)GetParent()).GlobalPosition;
        Vector2 direction = (CurrentPosition - PreviousPosition).Normalized();
        
        AddPoint(ToLocal(CurrentPosition - direction * 100));
        if (Points.Length > 30)
        {
            RemovePoint(0);
        }

        PreviousPosition = CurrentPosition;
    }
}
