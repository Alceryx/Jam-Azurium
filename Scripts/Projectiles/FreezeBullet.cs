using Godot;
using Godot.Collections;
using System;

public partial class FreezeBullet : Projectile
{
    [Export] private MeshInstance2D FreezeAreaDisplay;
    [Export] private CollisionShape2D FreezeArea;
    [Export] private Sprite2D Sprite;
    
    [Export] private float JumpHeight;
    [Export] private float TimeToApex;
    [Export] private float TimeToGround;
    private float Speed;

    private Vector2 NewVelocity;
    private Vector2 OldVelocity;
    private Vector2 AppliedVelocity;
    
    private bool IsJumping;
    private bool IsFalling;
    
    private float ApexGravity =>  2 * JumpHeight / (TimeToApex * TimeToApex);
    private float FallGravity => 2 * JumpHeight / (TimeToGround * TimeToGround);
    private float JumpForce => -2 * JumpHeight / TimeToApex;
    
    private float JumpTimer;

    private Array<Enemy> EnemyToFreeze = new();
    private bool CanFreeze;


    public override void _Ready()
    {
        WaveManager.WM.WaveStarted += Destroy;
        
        FreezeAreaDisplay.Hide();
        Sprite.Show();
        
        Speed = GlobalPosition.DistanceTo(Target) / (TimeToApex + TimeToGround);
        
        NewVelocity.Y = JumpForce;
    }

    
    public override void _PhysicsProcess(double delta)
    {
        if (CanFreeze)
        {
            Sprite.Hide();
            FreezeAreaDisplay.Show();
            foreach (Enemy enemy in EnemyToFreeze)
            {
                if (!enemy.Frozen)
                    enemy.Freeze();
            }
            FreezeArea.Disabled = true;
            return;
        }
        
        AppliedVelocity = Velocity;
        
        Move();

        OldVelocity = NewVelocity;
        NewVelocity.Y += Gravity() * (float)delta;
        NewVelocity.Y = Mathf.Clamp(NewVelocity.Y, !IsOnCeiling() ? JumpForce : 0, !IsOnFloor() ? Mathf.Inf : 0);

        AppliedVelocity.Y = (OldVelocity.Y + NewVelocity.Y) * .5f;
        
        Velocity = AppliedVelocity;
        MoveAndSlide();
    }

    public override void OnBodyEntered(Node2D body)
    {
        if (body is Enemy && !CanFreeze)
        {
            ((Enemy)body).TakeDamage(Damage);
            CanFreeze = true;
        }
    }

    void Move()
    {
        AppliedVelocity.X = Direction.X * Speed;
    }

    float Gravity()
    {
        IsFalling = !IsOnFloor() && NewVelocity.Y >= 0;
        return IsFalling ? FallGravity : ApexGravity;
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
        WaveManager.WM.WaveStarted -= Destroy;
        QueueFree();
    }
}

