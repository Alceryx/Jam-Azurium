using Godot;
using System;

public partial class SecondTurret : Turret
{
    [Export] private Marker2D ShootPoint;

    public override void Shoot(Vector2 target)
    {
        if (!CanShoot)
            return;
        
        if (Mode == 0)
        {
            Projectile projectile = Data.Modes[Mode].Projectile.Instantiate<Projectile>();
            
            UpdateSprite();
            
            Vector2 AimPos = target + TargetedEnemy.MoveDirection * TargetedEnemy.Speed;
            Vector2 direction = (AimPos - ShootPoint.GlobalPosition).Normalized();
            projectile.Setup(this, direction, Data.Modes[Mode].Damage, AimPos);
            projectile.GlobalPosition = ShootPoint.GlobalPosition;
            GameManager.GM.CallDeferred("add_child", projectile);
            
            SelfDestruct();
        }
        else
        {   
            
        }
        CanShoot = false;
    }

    public override void UpdateSprite()
    {
        SpriteState.Play("Top Right");
    }
}
