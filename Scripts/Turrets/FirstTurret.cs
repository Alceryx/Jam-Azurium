using Godot;
using System;

public partial class FirstTurret : Turret
{

    public override void Shoot(Vector2 target)
    {
        if (!CanShoot)
            return;
        
        if (Mode == 0)
        {
            Projectile projectile = Data.Modes[Mode].Projectile.Instantiate<Projectile>();
            
            float angle = GlobalPosition.GetIsometricAngleTo(target);
            
            if (angle >= 225 && angle < 315)
                Direction = FacingDirection.TopLeft;
            else if (angle >= 315 || angle < 45)
                Direction = FacingDirection.TopRight;
            else if (angle >= 135 && angle < 225)
                Direction = FacingDirection.BottomLeft;
            else if (angle >= 45 && angle < 135)
                Direction = FacingDirection.BottomRight;
            
            UpdateSprite();
            
            Vector2 direction = target - GlobalPosition;
            projectile.Setup(direction, Data.Modes[Mode].Damage);
            projectile.GlobalPosition = GlobalPosition;
            GameManager.GM.CallDeferred("add_child", projectile);
            
            SelfDestruct();
            CanShoot = false;
        }
        else
        {
            
        }
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body is Enemy)
            Shoot(body.GlobalPosition);
    }
}
