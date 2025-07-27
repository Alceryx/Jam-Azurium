using Godot;
using System;

public partial class BounceBullet : Projectile
{
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


    public override void _Ready()
    {
        
        Speed = GlobalPosition.DistanceTo(Target) / (TimeToApex + TimeToGround);
        
        NewVelocity.Y = JumpForce;
    }

    
    public override void _PhysicsProcess(double delta)
    {
        AppliedVelocity = Velocity;
        
        Move();

        OldVelocity = NewVelocity;
        NewVelocity.Y += Gravity() * (float)delta;
        NewVelocity.Y = Mathf.Clamp(NewVelocity.Y, !IsOnCeiling() ? JumpForce : 0, !IsOnFloor() ? Mathf.Inf : 0);

        AppliedVelocity.Y = (OldVelocity.Y + NewVelocity.Y) * .5f;
        
        Velocity = AppliedVelocity;
        MoveAndSlide();
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
}

