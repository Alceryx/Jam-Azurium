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
   [Export] public AnimationPlayer SpriteState;
   [Export] private Area2D DetectionArea;
   [Export] private CollisionShape2D DetectionShape;
   [Export] private MeshInstance2D DectectionPreview;
   [Export] private Shader HighlightShader;
   [Export] private float DestructDelay;
   private float DestructTimer;

   public FacingDirection Direction = FacingDirection.TopRight;
   public TurretData Data;
   public Enemy TargetedEnemy;
   public int Mode;

   public bool Preview;
   public bool CanClick;
   public bool Interacted;
   public bool CanShoot = true;
   
   public Array<Vector2I> OccupiedPositions;
   public Array<Enemy> EnemyToTarget = new();

   public abstract void Shoot(Vector2 target);

   public override void _Ready()
   {
      Sprite.Material = new ShaderMaterial();
      ((ShaderMaterial)Sprite.Material).SetShader(HighlightShader);
      ((ShaderMaterial)Sprite.Material).SetShaderParameter("outline_color", new Color(1,1,1,0));
      ((ShaderMaterial)Sprite.Material).SetShaderParameter("outline_thickness", 2);

      if (!Preview)
      {
         DetectionArea.BodyEntered += (body) => OnBodyEntered(body);
         DetectionArea.BodyExited += (body) => OnBodyExited(body); 
      }
   }

   public override void _Process(double delta)
   {
      if (EnemyToTarget.Count > 0)
         {
            while (EnemyToTarget.Count > 0 && EnemyToTarget[EnemyToTarget.Count - 1].Targeted)
               EnemyToTarget.RemoveAt(EnemyToTarget.Count - 1);

            float MinDist = Mathf.Inf;
            foreach (Enemy enemy in EnemyToTarget)
            {
               if (enemy.GlobalPosition.DistanceTo(GlobalPosition) < MinDist)
               {
                  MinDist = enemy.GlobalPosition.DistanceTo(GlobalPosition);
                  TargetedEnemy = enemy;
               }
            }

            if (IsInstanceValid(TargetedEnemy))
            {
               TargetedEnemy.Targeted = true;
               Shoot(TargetedEnemy.GlobalPosition);
            }
         }
      
      if (DestructTimer > 0)
      {
         DestructTimer -= (float)delta;
         if (DestructTimer <= 0)
         {
            BuildManager.BM.Destroy(this);
         }
      }
      
      if (Input.IsActionJustPressed("Interact") && CanClick && !BuildManager.BM.IsBuilding)
      {
         Interacted = true;
         ShowDetectionPreview();
      }

      if (!Preview && BuildManager.BM.IsBuilding || (Input.IsActionJustReleased("Interact") && !CanClick))
      {
         Interacted = false;
         HideDetectionPreview();
         Tween tween = CreateTween();
         tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1, 0), 0.15f);
         CanClick = false;
      }
      
      if (Input.IsActionJustPressed("Destroy") && Interacted)
         BuildManager.BM.Store(this);
      
      if (Input.IsActionJustPressed("Move") && Interacted)
         BuildManager.BM.Move(this);
   }

   public void SetUp(TurretData Data, int Mode, FacingDirection Direction, Array<Vector2I> OccupiedPositions = null, bool Preview = false)
   {
      this.Data = Data;
      this.Mode = Mode;
      this.Direction = Direction;
      this.Preview = Preview;
      this.OccupiedPositions = OccupiedPositions;

      Sprite.Texture = Data.Modes[Mode].Icon;
      ((CircleShape2D)DetectionShape.Shape).SetRadius(Data.Modes[Mode].Range);

      UpdateSprite();

      DetectionShape.Disabled = Preview;
   }


   public virtual void UpdateSprite()
   {
      switch (Direction)
      {
         case FacingDirection.TopLeft:
            SpriteState.Play("Top Left");
            break;
         case FacingDirection.TopRight:
            SpriteState.Play("Top Right");
            break;
         case FacingDirection.BottomLeft:
            SpriteState.Play("Bottom Left");
            break;
         case FacingDirection.BottomRight:
            SpriteState.Play("Bottom Right");
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
   
   private void OnMouseEntered()
   {
      if (Preview)
         return;
      
      Tween tween = CreateTween();
      tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1), 0.15f);
      tween.Parallel().TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.15f);
      CanClick = true;
   }

   private void OnMouseExited()
   {
      CanClick = false;
      if (Preview || Interacted)
         return;
      
      Tween tween = CreateTween();
      tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1, 0), 0.15f);
      tween.Parallel().TweenProperty(this, "scale", new Vector2(1, 1), 0.15f);
   }
   
   private void OnBodyEntered(Node2D body)
   {
      if (body is Enemy)
      {
         Enemy enemy = (Enemy)body;
         EnemyToTarget.Add(enemy);
      }
   }

   private void OnBodyExited(Node2D body)
   {
      if (body is Enemy && EnemyToTarget.Contains((Enemy)body))
      {
         EnemyToTarget.Remove((Enemy)body);
      }

      if (body is Enemy && TargetedEnemy == (Enemy)body)
      {
         ((Enemy)body).Targeted = false;
      }
   }
}
