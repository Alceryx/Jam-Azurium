using Godot;
using System;

public partial class Voltspire : Turret
{
    [Export] private Marker2D ShootPoint;
    [Export] private Line2D Line;
    
    public override void Shoot(Vector2 target)
    {
        if (!CanShoot)
            return;
        
        Projectile projectile = Data.Modes[Mode].Projectile.Instantiate<Projectile>();
        
        UpdateSprite(); 
        projectile.Setup(this, Vector2.Up, Data.Modes[Mode].Damage, TargetedEnemy.GlobalPosition, TargetedEnemy);
        projectile.GlobalPosition = ShootPoint.GlobalPosition;
        GameManager.GM.CallDeferred("add_child", projectile);

        projectile.Hit += SelfDestruct;

        if (Mode == 0)
        {
            projectile = Data.Modes[Mode].Projectile.Instantiate<Projectile>();
            projectile.Setup(this, Vector2.Down, Data.Modes[Mode].Damage, TargetedEnemy.GlobalPosition, TargetedEnemy);
            projectile.GlobalPosition = ShootPoint.GlobalPosition;
            GameManager.GM.CallDeferred("add_child", projectile);
        }

        
        projectile.Hit += SelfDestruct;

        if (Mode == 0)
        {
            Line.Show();
            SpriteState.Play("Mode 1 Shoot");
        }
        else
        {
            Line.Show();
            SpriteState.Play("Mode 2 Shoot");
        }
        
        CanShoot = false;
    }

    public override void UpdateSprite()
    {
        if (Mode == 0)
            SpriteState.Play("Mode 1");
        else
            SpriteState.Play("Mode 2");
    }
}
