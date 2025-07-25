using Godot;
using System;

public partial class Bullet : Projectile
{
    [Export] private float Speed;
    
    public override void _PhysicsProcess(double delta)
    {
        Velocity = Direction * Speed;
        LookAt(Direction * 100);
        MoveAndSlide();
    }
}
