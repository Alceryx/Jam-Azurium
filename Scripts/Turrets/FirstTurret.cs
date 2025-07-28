using Godot;
using System;

public partial class FirstTurret : Turret
{
    [Export] private Marker2D OriginPoint;
    [Export] private Marker2D TopLeftShootPoint;
    [Export] private Marker2D TopRightShootPoint;
    [Export] private Marker2D BottomLeftShootPoint;
    [Export] private Marker2D BottomRightShootPoint;
    
    public override void _Ready()
    {
        base._Ready();
        
        if (!Preview && Mode == 1)
        {
            Shoot(Vector2.Zero);
            WaveManager.WM.WaveEnded += OnWaveEnded;
        }
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
            GameManager.GM.CallDeferred("add_child", projectile);
            
            SelfDestruct();
        }
        else
        {
            Projectile projectile = Data.Modes[Mode].Projectile.Instantiate<Projectile>();
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
}
