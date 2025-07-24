using Godot;
using System;

public partial class Turret : StaticBody2D
{
   public enum FacingDirection
   {
      TopRight,
      BottomRight,
      BottomLeft,
      TopLeft
   }
   
   [Export] public Sprite2D Sprite;

   public FacingDirection Direction = FacingDirection.TopRight;
   public TurretData Data;
   public int Mode;


   public void SetUp(TurretData Data, int Mode, FacingDirection Direction)
   {
      this.Data = Data;
      this.Mode = Mode;
      this.Direction = Direction;

      Sprite.Texture = Data.Modes[Mode].Icon;

      if (Data.Modes[Mode].CanRotate)
      {
         switch (Direction)
         {
            case FacingDirection.TopLeft:
               Sprite.Texture = Data.Modes[Mode].TopLeft;
               break;
            case FacingDirection.TopRight:
               Sprite.Texture = Data.Modes[Mode].TopRight;
               break;
            case FacingDirection.BottomLeft:
               Sprite.Texture = Data.Modes[Mode].BottomLeft;
               break;
            case FacingDirection.BottomRight:
              Sprite.Texture = Data.Modes[Mode].BottomRight;
               break;
         }
      }
      
   }
   
   private void Shoot()
   {
        
   }
}
