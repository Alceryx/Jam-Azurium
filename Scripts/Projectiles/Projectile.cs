using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
    [Export] private float Speed;
    private float Damage;
    private Vector2 Direction;
    
    public void Setup(Vector2 Direction, float Damage)
    {
        this.Direction = Direction;
        this.Damage = Damage;
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = Direction * Speed * (float)delta;
        LookAt(Direction * 100);
        MoveAndSlide();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Enemy)
        {
            ((Enemy)body).TakeDamage(Damage);
            QueueFree();
        }
    }
}
