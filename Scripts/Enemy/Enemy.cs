using Godot;
using Godot.Collections;
using System;

public partial class Enemy : CharacterBody2D
{
    public enum FacingDirection
    {
        Front,
        Back,
        Left,
        Right
    }
    
    [ExportGroup("Data")]
    [Export] private float Speed = 100.0f;
    [Export] private Texture2D FrontSprite;
    [Export] private Texture2D BackSprite;
    [Export] private Texture2D LeftSprite;
    [Export] private Texture2D RightSprite;

    [ExportGroup("References")] 
    [Export] private Sprite2D Sprite;
    
    private Path2D Path;
    private PathFollow2D PathFollow;
    private Vector2 LastPosition;
    private FacingDirection Direction;
    
    public void Setup(Path2D Path)
    {
        this.Path = Path;
        
        PathFollow = new PathFollow2D();
        PathFollow.Progress = 0;
        PathFollow.Loop = false;
        PathFollow.Rotates = false;
        Path.AddChild(PathFollow);
        
        GlobalPosition = PathFollow.GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(Path) || !IsInstanceValid(PathFollow))
            return;

        // Move toward current target
        PathFollow.Progress += Speed * (float)delta;
        GlobalPosition = PathFollow.GlobalPosition;
        UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        Vector2 movementVector = GlobalPosition - LastPosition;
        LastPosition = GlobalPosition;

        Vector2 isoVector = CartesianToIsometric(movementVector);

        float angle = Mathf.RadToDeg(isoVector.Angle());
        if (angle < 0)
            angle += 360;
        
        if (angle >= 225 && angle < 315)
            Direction = FacingDirection.Front;
        else if (angle >= 315 || angle < 45)
            Direction = FacingDirection.Right;
        else if (angle >= 45 && angle < 135)
            Direction = FacingDirection.Back;
        else if (angle >= 135 && angle < 225)
            Direction = FacingDirection.Left;
        

        UpdateSprite();
    }

    private Vector2 CartesianToIsometric(Vector2 vector)
    {
        return new Vector2(
            vector.X - vector.Y,
            (vector.X + vector.Y) / 2f
        );
    }

    private void UpdateSprite()
    {
        switch (Direction)
        {
            case FacingDirection.Front:
                Sprite.Texture = FrontSprite;
                break;
            case FacingDirection.Back:
                Sprite.Texture = BackSprite;
                break;
            case FacingDirection.Left:
                Sprite.Texture = LeftSprite;
                break;
            case FacingDirection.Right:
                Sprite.Texture = RightSprite;
                break;
            
        }
    }
}
