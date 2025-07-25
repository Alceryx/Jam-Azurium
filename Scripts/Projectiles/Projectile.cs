using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
    [Export] public Area2D Hitbox;
    [Export] public bool Pierce;
    public float Damage;
    public Vector2 Direction;
    public Turret Turret;
    

    public void Setup(Turret Turret, Vector2 Direction, float Damage)
    {
        this.Direction = Direction;
        this.Damage = Damage;
        this.Turret = Turret;
        
        Hitbox.BodyEntered += (Node2D body) => OnBodyEntered(body);
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body is Enemy)
        {
            ((Enemy)body).TakeDamage(Damage);
            if (!Pierce)
                QueueFree();
        }
    }
}
