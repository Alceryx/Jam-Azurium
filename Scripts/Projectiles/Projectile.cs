using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
    [Export] public Area2D Hitbox;
    [Export] public bool Pierce;
    [Export] public float LifeTime;
    public float Damage;
    public Vector2 Direction;
    public Vector2 Target;
    public Turret Turret;

    [Signal]
    public delegate void HitEventHandler();
    

    public void Setup(Turret Turret, Vector2 Direction, float Damage, Vector2 Target)
    {
        this.Direction = Direction;
        this.Damage = Damage;
        this.Turret = Turret;
        this.Target = Target;
        
        Hitbox.BodyEntered += (Node2D body) => OnBodyEntered(body);
    }

    public override void _Process(double delta)
    {
        if (LifeTime > 0)
        {
            LifeTime -= (float)delta;
            if (LifeTime <= 0)
            {
                QueueFree();
                EmitSignalHit();
            }
        }
    }

    public virtual void OnBodyEntered(Node2D body)
    {
        if (body is Enemy)
        {
            ((Enemy)body).TakeDamage(((Enemy)body).MaxHealth * Damage);
            if (!Pierce)
            {
                EmitSignalHit();
                QueueFree();
            }
        }
    }
}
