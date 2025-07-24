using Godot;
using System;

public partial class Turret : StaticBody2D
{
   [Export] public Sprite2D Sprite;
   
   public TurretData Data;
   public int Mode;


   public void SetUp(TurretData Data, int Mode)
   {
      this.Data = Data;
      this.Mode = Mode;

      Sprite.Texture = Data.Modes[Mode].Icon;
   }
   
   public void Shoot()
   {
      
   }
}
