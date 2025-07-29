using Godot;
using System;

public partial class NexusButton : StaticBody2D
{
    [Export] private int MaxCPS;
    [Export] private float HoldThreshold;
    [Export] private AnimationPlayer SpriteState;
    [Export] private Sprite2D Sprite;
    private bool CanClick;
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
    }

    public override void _Process(double delta)
    {
        if (Releasing && RiseTimer > 0)
        {
            SpriteState.Play("Mid");
            RiseTimer -= (float)delta;
        }
        else if (!Releasing) RiseTimer = RiseDelay;
        if (RiseTimer <= 0) SpriteState.Play("Neutral");
        
        if (ClickTimer > 0)
        {
            ClickTimer -= (float)delta;
        }

        if (HoldTimer >= HoldThreshold && WaveManager.WM.WaveFinished)
        {
            ShopManager.SM.Show();
            HoldTimer = 0;
        }

        if (!BuildManager.BM.IsBuilding && CanClick && ClickTimer <= 0)
        {
            SpriteState.Play("Mid");
            if (Input.IsActionJustPressed("Interact"))
            {
                ClickTimer = ClickDelay;
                GameManager.GM.Currency += GameManager.GM.Efficiency;
                
                SpriteState.Play("Clicked");
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
        GD.Print(GameManager.GM.ButtonHP);
    }
    private void OnMouseEntered()
    {
        Releasing = false;
        CanClick = true;
    }

    private void OnMouseExited()
    {
        Releasing = true;
        CanClick = false;
    }

}
