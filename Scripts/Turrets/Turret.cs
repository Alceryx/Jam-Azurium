using Godot;
using Godot.Collections;
using System;

public abstract partial class Turret : StaticBody2D
{
   public enum FacingDirection
   {
      TopRight,
      BottomRight,
      BottomLeft,
      TopLeft
   }
   
   [Export] public Sprite2D Sprite;
   [Export] private CollisionShape2D DetectionArea;
   [Export] private MeshInstance2D DectectionPreview;
   [Export] private float DestructDelay;
   private float DestructTimer;

   public FacingDirection Direction = FacingDirection.TopRight;
   public TurretData Data;
   public int Mode;

   public bool Preview;
   public bool CanShoot = true;
   
   private Array<Vector2I> OccupiedPositions;

   public abstract void Shoot(Vector2 target);

   public override void _Process(double delta)
   {
      if (DestructTimer > 0)
      {
         DestructTimer -= (float)delta;
         if (DestructTimer <= 0)
         {
            foreach (Vector2I pos in OccupiedPositions)
            {
               BuildManager.BM.OccupiedTiles.Remove(pos);
            }
            BuildManager.BM.PlaceableLayer.EraseCell(OccupiedPositions[0]);
         }
      }
   }

   public void SetUp(TurretData Data, int Mode, FacingDirection Direction, Array<Vector2I> OccupiedPositions = null, bool Preview = false)
   {
      this.Data = Data;
      this.Mode = Mode;
      this.Direction = Direction;
      this.Preview = Preview;
      this.OccupiedPositions = OccupiedPositions;

      Sprite.Texture = Data.Modes[Mode].Icon;
      ((CircleShape2D)DetectionArea.Shape).SetRadius(Data.Modes[Mode].Range);

      if (Data.Modes[Mode].CanRotate)
      {
         UpdateSprite();
      }

      DetectionArea.Disabled = Preview;
   }


   public void UpdateSprite()
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

   public void SelfDestruct()
   {
       DestructTimer = DestructDelay;
   }

   public void ShowDetectionPreview()
   {
      DectectionPreview.Show();
      SphereMesh mesh = new SphereMesh();
      mesh.SetRadius(Data.Modes[Mode].Range);
      mesh.SetHeight(Data.Modes[Mode].Range * 2);
      DectectionPreview.SetMesh(mesh);
   }

   public void HideDetectionPreview()
   {
      DectectionPreview.Hide();
   }
}
