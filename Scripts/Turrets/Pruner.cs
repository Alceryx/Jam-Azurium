using Godot;
using System;
using Godot.Collections;

public partial class Pruner : Turret
{
    [Export] private Marker2D OriginPoint;
    [Export] private Marker2D TopLeftShootPoint;
    [Export] private Marker2D TopRightShootPoint;
    [Export] private Marker2D BottomLeftShootPoint;
    [Export] private Marker2D BottomRightShootPoint;
    private Array<RayCast2D> DirCast = new();

    private int EnemyEnteredLaser = 0;
    
    public override void _Ready()
    {
        base._Ready();
        
        if (!Preview && Mode == 1)
        {
            Shoot(Vector2.Zero);
            WaveManager.WM.WaveEnded += OnWaveEnded;
        }

        if (!Preview && Mode == 0)
        {
            DetectionShape.Disabled = true;
            Array<Vector2> ShootPoints = new Array<Vector2>()
            {
                TopLeftShootPoint.GlobalPosition,
                TopRightShootPoint.GlobalPosition,
                BottomLeftShootPoint.GlobalPosition,
                BottomRightShootPoint.GlobalPosition
            };

            foreach (var point in ShootPoints)
            {
                RayCast2D ray = new RayCast2D();
                AddChild(ray);
                ray.GlobalPosition = OriginPoint.GlobalPosition + new Vector2(0, 100);
                ray.TargetPosition = (point - OriginPoint.GlobalPosition).Normalized() * (30 + Data.Modes[Mode].Range * 70);
                DirCast.Add(ray);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (Mode == 0)
        {
            foreach (var ray in DirCast)
            {
                if (ray.GetCollider() is Enemy)
                {
                    Enemy enemy = (Enemy)ray.GetCollider();
                    if (CanShoot && !enemy.Targeted && !enemy.Frozen)
                    {
                        TargetedEnemy = enemy;
                        enemy.Targeted = true;
                        Shoot(ray.GetCollisionPoint());
                    }
                }
            }
        }
        base._Process(delta);
    }
    
    public override void Shoot(Vector2 target)
    {
        if (!CanShoot)
            return;
        
        if (Mode == 0)
        {
            Projectile projectile = Data.Modes[Mode].Projectile.Instantiate<Projectile>();
            
            float angle = GlobalPosition.GetIsometricAngleTo(target);
            Vector2 ShootPoint = new();

            if (angle >= 225 && angle < 315)
            {
                Direction = FacingDirection.TopLeft;
                ShootPoint = TopLeftShootPoint.GlobalPosition;
            }
            else if (angle >= 315 || angle < 45)
            {
                Direction = FacingDirection.TopRight;
                ShootPoint = TopRightShootPoint.GlobalPosition;
            }
            else if (angle >= 135 && angle < 225)
            {
                Direction = FacingDirection.BottomLeft;
                ShootPoint = BottomLeftShootPoint.GlobalPosition;
            }
            else if (angle >= 45 && angle < 135)
            {
                Direction = FacingDirection.BottomRight;
                ShootPoint = BottomRightShootPoint.GlobalPosition;
            }
            
            UpdateSprite();
            
            Vector2 direction = (target - ShootPoint).Normalized();
            projectile.Setup(this, direction, Data.Modes[Mode].Damage, target);
            projectile.GlobalPosition = ShootPoint;
            projectile.Hit += OnProjectileHit;
            GameManager.GM.CallDeferred("add_child", projectile);
        }
        else
        {
            Projectile projectile = Data.Modes[Mode].Projectile.Instantiate<Projectile>();
            projectile.Hitbox.BodyEntered += (body) => OnLaserEntered(body);
            
            Vector2 ShootPoint = new();
            
            if (Direction == FacingDirection.TopLeft)
                ShootPoint = TopLeftShootPoint.GlobalPosition;
            else if (Direction == FacingDirection.TopRight)
                ShootPoint = TopRightShootPoint.GlobalPosition;
            else if (Direction == FacingDirection.BottomLeft)
                ShootPoint = BottomLeftShootPoint.GlobalPosition;
            else if (Direction == FacingDirection.BottomRight)
                ShootPoint = BottomRightShootPoint.GlobalPosition;
            
            Vector2 direction = (ShootPoint - OriginPoint.GlobalPosition).Normalized();
            projectile.Setup(this, direction, Data.Modes[Mode].Damage, target);
            projectile.GlobalPosition = ShootPoint;
            GameManager.GM.CallDeferred("add_child", projectile);

        }
        CanShoot = false;
    }

    private void OnWaveEnded()
    {
        SelfDestruct();
    }

    private void OnLaserEntered(Node2D body)
    {
        if (body is Enemy)
        {
            EnemyEnteredLaser++;
            if (EnemyEnteredLaser >= 3)
                SelfDestruct();
        }
    }

    private void OnProjectileHit()
    {
        if (Mode == 0)
            SelfDestruct();
    }
}
