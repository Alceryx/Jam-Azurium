using Godot;
using System;

public partial class Laser : Projectile
{
    [Export] private float CastSpeed;
    [Export] private RayCast2D RayCast;
    [Export] private Line2D Line;

    private SeparationRayShape2D Ray;
    private CollisionShape2D Collider;

    public override void _Ready()
    {
        Collider = Hitbox.GetChild<CollisionShape2D>(0);
        Ray = new SeparationRayShape2D();
        Collider.SetShape(Ray);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(Turret))
            QueueFree();
        RayCast.TargetPosition = RayCast.TargetPosition.MoveToward(Direction * 10000, CastSpeed * (float)delta);
        Ray.SetLength(GlobalPosition.DistanceTo(ToGlobal(RayCast.TargetPosition)));
        Collider.SetRotation((ToGlobal(RayCast.TargetPosition) - GlobalPosition).Normalized().Angle() - Mathf.Pi / 2);
        Vector2 EndPoint = RayCast.TargetPosition;
        RayCast.ForceRaycastUpdate();
        if (RayCast.IsColliding())
            EndPoint = ToLocal(RayCast.GetCollisionPoint());
        Line.SetPointPosition(1, EndPoint);
    }

    public override void OnBodyEntered(Node2D body)
    {
        if (body is Enemy)
        {
            ((Enemy)body).TakeDamage(((Enemy)body).MaxHealth / 2);
        }
    }
}
