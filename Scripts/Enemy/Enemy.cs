using Godot;
using Godot.Collections;
using System;

public partial class Enemy : CharacterBody2D
{
    public enum FacingDirection
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [ExportGroup("Data")] 
    [Export] private float Health = 5f;
    [Export] public float Speed = 100.0f;
    [Export] private float Damage = 5f;

    [ExportGroup("References")] 
    [Export] private AnimationPlayer sprite;
    
    private Path2D Path;
    private PathFollow2D PathFollow;
    private Vector2 LastPosition;
    public Vector2 MoveDirection;
    private FacingDirection Direction;

    public bool Targeted;
    
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

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            QueueFree();
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
        float angle = LastPosition.GetIsometricAngleTo(GlobalPosition);
        MoveDirection = (GlobalPosition - LastPosition).Normalized();
        LastPosition = GlobalPosition;
        
        if (angle >= 225 && angle < 315)
            Direction = FacingDirection.TopLeft;
        else if (angle >= 315 || angle < 45)
            Direction = FacingDirection.TopRight;
        else if (angle >= 135 && angle < 225)
            Direction = FacingDirection.BottomLeft;
        else if (angle >= 45 && angle < 135)
            Direction = FacingDirection.BottomRight;
        

        UpdateSprite();
    }

    private Vector2 CartesianToIsometric(Vector2 vector) => new(vector.X - vector.Y, (vector.X + vector.Y) / 2f);

    private void UpdateSprite()
    {
        switch (Direction)
        {
            case FacingDirection.TopLeft:
                sprite.Play("Top Left");
                break;
            case FacingDirection.TopRight:
                sprite.Play("Top Right");
                break;
            case FacingDirection.BottomLeft:
                sprite.Play("Bottom Left");
                break;
            case FacingDirection.BottomRight:
                sprite.Play("Bottom Right");
                break;
            
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is NexusButton)
        {
            ((NexusButton)body).TakeDamage(Damage);
            QueueFree();
        }
    }
}
