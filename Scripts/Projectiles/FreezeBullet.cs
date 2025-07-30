using Godot;
using Godot.Collections;
using System;

public partial class FreezeBullet : Projectile
{
    [ExportGroup("Info")]
    [Export] private float Speed;
    [Export] private float HoverTime;
    [Export] private float HoverHeight;
    [ExportGroup("Freeze")]
    [Export] private CollisionShape2D FreezeArea;
    
    private Array<Enemy> EnemyToFreeze = new();
    private bool CanFreeze;
    private float HoverTimer;
    private bool Hovered;

    private Vector2 OriginalPosition;

    public override void _Ready()
    {
        HoverTimer = HoverTime;
        OriginalPosition = GlobalPosition;
        
        WaveManager.WM.WaveStarted += Destroy;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CanFreeze)
        {
            foreach (Enemy enemy in EnemyToFreeze)
            {
                if (!enemy.Frozen)
                    enemy.Freeze();
            }
            FreezeArea.Disabled = true;
            return;
        }
        
        if (HoverTimer > 0)
        {
            HoverTimer -= (float)delta;
            if (!Hovered)
            {
                Velocity = Vector2.Up * Speed;
                if (GlobalPosition.DistanceTo(OriginalPosition) >= HoverHeight)
                {
                    if (Direction == Vector2.Up)
                        Direction = (Targeted.GlobalPosition - GlobalPosition).Normalized();
                    else
                        Direction = (GlobalPosition - Targeted.GlobalPosition).Normalized();
                    Hovered = true;
                }
            }
            else
            {
                Velocity = Vector2.Zero;
            }
        }
        else
        {
            Velocity = Direction * Speed;
            LookAt(Direction * 10000);
        }
        MoveAndSlide();
    }
    
    
    public override void OnBodyEntered(Node2D body)
    {
        if (body is Enemy && !CanFreeze)
        {
            ((Enemy)body).TakeDamage(Damage);
            EmitSignalHit();
            CanFreeze = true;
        }
    }
    
    private void OnFreezeEntered(Node2D body)
    {
        if (body is Enemy)
            EnemyToFreeze.Add((Enemy)body);
    }

    private void OnFreezeExited(Node2D body)
    {
        if (body is Enemy)
            EnemyToFreeze.Remove((Enemy)body);
    }

    private void Destroy()
    {
        if (IsInstanceValid(this))
        {
            WaveManager.WM.WaveStarted -= Destroy;
            QueueFree();
        }
        
    }
}

