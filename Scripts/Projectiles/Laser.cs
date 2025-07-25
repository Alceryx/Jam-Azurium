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
        RayCast.TargetPosition = RayCast.TargetPosition.MoveToward(Direction * Turret.Data.Modes[Turret.Mode].Range, CastSpeed * (float)delta);
        Ray.SetLength(GlobalPosition.DistanceTo(ToGlobal(RayCast.TargetPosition)));
        Collider.SetRotation((ToGlobal(RayCast.TargetPosition) - GlobalPosition).Normalized().Angle() - Mathf.Pi / 2);
        Vector2 EndPoint = RayCast.TargetPosition;
        RayCast.ForceRaycastUpdate();
        if (RayCast.IsColliding())
            EndPoint = ToLocal(RayCast.GetCollisionPoint());
        Line.SetPointPosition(1, EndPoint);
    }
}
