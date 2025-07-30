using Godot;
using System;

public partial class NexusButton : StaticBody2D
{
    [Export] private int MaxCPS;
    [Export] private float HoldThreshold;
    [Export] private AnimationPlayer SpriteState;
    [Export] private Sprite2D Sprite;
    [Export] private PackedScene Particle;
    public bool CanClick;
    private bool Releasing;

    private float ClickDelay;
    private float ClickTimer;
    private float HoldTimer;

    private float RiseDelay = .3f;
    private float RiseTimer;

    public override void _Ready()
    {
        SpriteState.Play("Neutral");
        ClickDelay = 1.0f / MaxCPS;
        
        CallDeferred("SetInstance");
    }

    public override void _Process(double delta)
    {
        if (WaveManager.WM.WaveFinished && WaveManager.WM.CountDownFinished)
        {
            if (Releasing && RiseTimer > 0)
            {
                SpriteState.Play("Mid");
                RiseTimer -= (float)delta;
            }
            else if (!Releasing) RiseTimer = RiseDelay;
            if (RiseTimer <= 0) SpriteState.Play("Neutral");
        }
        else
        {
            SpriteState.Play("Neutral");
        }
        
        if (ClickTimer > 0)
        {
            ClickTimer -= (float)delta;
        }

        if (HoldTimer >= HoldThreshold && WaveManager.WM.WaveFinished && WaveManager.WM.CountDownFinished)
        {
            ShopManager.SM.Show();
            HoldTimer = 0;
        }

        if (!BuildManager.BM.IsBuilding && CanClick && ClickTimer <= 0 && WaveManager.WM.WaveFinished && WaveManager.WM.CountDownFinished)
        {
            SpriteState.Play("Mid");
            if (Input.IsActionJustPressed("Interact"))
            {
                ClickTimer = ClickDelay;
                GameManager.GM.Currency += GameManager.GM.Efficiency;
                
                SpawnCloud();
                if (ShopManager.SM.Visible) ShopManager.SM.Hide();
            }

            if (Input.IsActionPressed("Interact") )
            {
                SpriteState.Play("Clicked");
                HoldTimer += (float)delta;
            }

            if (Input.IsActionJustReleased("Interact"))
            {
                HoldTimer = 0;
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        GameManager.GM.ButtonHP -= damage;
    }

    private void SpawnCloud()
    {
        GpuParticles2D SmokeCloud = Particle.Instantiate() as GpuParticles2D;
        AddChild(SmokeCloud);
        MoveChild(SmokeCloud, 0);
        SmokeCloud.Position = new Vector2(0, 64);
        SmokeCloud.Emitting = true;
    }
    private void OnMouseEntered()
    {
        Releasing = false;
        CanClick = true;
        ShopManager.SM.IsInArea = false;
    }

    private void OnMouseExited()
    {
        Releasing = true;
        CanClick = false;
    }

    private void SetInstance()
    {
        GameManager.GM.NexusButton = this;
    }

}
