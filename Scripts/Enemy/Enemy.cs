using Godot;
using Godot.Collections;
using System;

public partial class Enemy : CharacterBody2D
{
    private enum FacingDirection
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [ExportGroup("Data")] 
    [Export] public float MaxHealth = 5f;
    [Export] public float Speed = 100.0f;
    [Export] private float Damage = 5f;
    public float CurrentHealth;

    [ExportGroup("References")] 
    [Export] private AnimationPlayer sprite;
    
    private Path2D Path;
    private PathFollow2D PathFollow;
    private Vector2 LastPosition;
    public Vector2 MoveDirection;
    private FacingDirection Direction;

    public bool Targeted;
    public bool Frozen;
    
    public void Setup(Path2D Path)
    {
        this.Path = Path;
        
        PathFollow = new PathFollow2D();
        PathFollow.Progress = 0;
        PathFollow.Loop = false;
        PathFollow.Rotates = false;
        Path.AddChild(PathFollow);
        
        GlobalPosition = PathFollow.GlobalPosition - new Vector2(0, 64);
    }

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        WaveManager.WM.WaveStarted += UnFreeze;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            WaveManager.WM.WaveStarted -= UnFreeze;
            QueueFree();
        }
    }

    public void Freeze()
    {
        Frozen = true;
        Modulate = new Color(0, 0, 1);
        RemoveFromGroup("Enemy");
    }

    private void UnFreeze()
    {
        if (Frozen && IsInstanceValid(this))
        {
            Frozen = false;
            Modulate = new Color(1, 1, 1);
            AddToGroup("Enemy");
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        
        if (!IsInstanceValid(Path) || !IsInstanceValid(PathFollow) || Frozen)
            return;

        // Move toward current target
        PathFollow.Progress += Speed * (float)delta;
        GlobalPosition = PathFollow.GlobalPosition - new Vector2(0, 64);
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
            WaveManager.WM.WaveStarted -= UnFreeze;
            QueueFree();
        }
    }
}
