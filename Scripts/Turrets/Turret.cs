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
   
   [ExportGroup("References")]
   [Export] public Sprite2D Sprite;
   [Export] public AnimationPlayer SpriteState;
   [Export] private Area2D DetectionArea;
   [Export] public CollisionShape2D DetectionShape;
   [Export] private MeshInstance2D DectectionPreview;
   [ExportGroup("Visual")]
   [Export] private Shader HighlightShader;
   [Export] private Shader DissolveShader;
   [Export] private float DestructDelay;
   [Export] private float HoldThreshold = 0.5f;
   private float DestructTimer;
   private float HoldTimer;

   public FacingDirection Direction = FacingDirection.TopRight;
   public TurretData Data;
   public Enemy TargetedEnemy;
   public int Mode;

   public bool Preview;
   public bool CanClick;
   public bool Interacted;
   public bool CanShoot = true;
   
   public Array<Vector2I> OccupiedPositions;

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
         TreeExited += OnTreeExited; 
      }
      
      WaveManager.WM.WaveEnded += SelfDestruct;
   }

   public override void _Process(double delta)
   {
      if (DestructTimer > 0)
      {
         ((ShaderMaterial)Sprite.Material).SetShaderParameter("dissolve_value", DestructTimer / DestructDelay);
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

      if (Input.IsActionPressed("Interact") && CanClick && !BuildManager.BM.IsBuilding && WaveManager.WM.WaveFinished)
      {
         HoldTimer += (float)delta;
         if (HoldTimer >= HoldThreshold)
         {
            HoldTimer = 0;
            BuildManager.BM.Move(this);
         }
      }

      if (Input.IsActionJustReleased("Interact"))
         HoldTimer = 0;

      if (!Preview && BuildManager.BM.IsBuilding || (Input.IsActionJustReleased("Interact") && !CanClick))
      {
         Interacted = false;
         HideDetectionPreview();
         Tween tween = CreateTween();
         tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1, 0), 0.15f);
         CanClick = false;
      }
      
      if (Input.IsActionJustPressed("Destroy") && Interacted && WaveManager.WM.WaveFinished)
         BuildManager.BM.Store(this);
   }

   public void SetUp(TurretData Data, int Mode, FacingDirection Direction, Array<Vector2I> OccupiedPositions = null, bool Preview = false)
   {
      this.Data = Data;
      this.Mode = Mode;
      this.Direction = Direction;
      this.Preview = Preview;
      this.OccupiedPositions = OccupiedPositions;

      Sprite.Texture = Data.Modes[Mode].Icon;
      ((CircleShape2D)DetectionShape.Shape).SetRadius(30 + Data.Modes[Mode].Range * 70);

      UpdateSprite();

      DetectionShape.Disabled = Preview;
   }


   public virtual void UpdateSprite()
   {
      switch (Direction)
      {
         case FacingDirection.TopLeft:
            if (Mode == 0)
               SpriteState.Play("Mode 1/Top Left");
            else
               SpriteState.Play("Mode 2/Top Left");
            break;
         case FacingDirection.TopRight:
            if (Mode == 0)
               SpriteState.Play("Mode 1/Top Right");
            else
               SpriteState.Play("Mode 2/Top Right");
            break;
         case FacingDirection.BottomLeft:
            if (Mode == 0)
               SpriteState.Play("Mode 1/Bottom Left");
            else
               SpriteState.Play("Mode 2/Bottom Left");
            break;
         case FacingDirection.BottomRight:
            if (Mode == 0)
               SpriteState.Play("Mode 1/Bottom Right");
            else
               SpriteState.Play("Mode 2/Bottom Right");
            break;
      }
   }

   public void SelfDestruct()
   {
      if (IsInstanceValid(Sprite))
      {
          ShaderMaterial material = (ShaderMaterial)Sprite.Material;
          NoiseTexture2D noise = new NoiseTexture2D();
          noise.SetNoise(new FastNoiseLite());
          material.SetShader(DissolveShader);
          material.SetShaderParameter("dissolve_texture", noise);
          DestructTimer = DestructDelay;
      }
   }

   public void ShowDetectionPreview()
   {
      DectectionPreview.Show();
      SphereMesh mesh = new SphereMesh();
      mesh.SetRadius(30 + Data.Modes[Mode].Range * 70);
      mesh.SetHeight(mesh.Radius * 2);
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
      CanClick = true;
   }

   private void OnMouseExited()
   {
      CanClick = false;
      if (Preview || Interacted)
         return;
      
      Tween tween = CreateTween();
      tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1, 0), 0.15f);
   }
   
   private void OnBodyEntered(Node2D body)
   {
      if (body is Enemy)
      {
         Enemy enemy = (Enemy)body;
         
         if (CanShoot && !enemy.Targeted && !enemy.Frozen)
         {
            TargetedEnemy = enemy;
            enemy.Targeted = true;
            Shoot(enemy.GlobalPosition);
         }
      }
   }

   private void OnTreeExited()
   {
      if (IsInstanceValid(TargetedEnemy))
         TargetedEnemy.Targeted = false;
   }
}
