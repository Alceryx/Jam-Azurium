using Godot;
using System;

public partial class NexusButton : StaticBody2D
{
    [Export] private int MaxCPS;
    [Export] private float HoldThreshold;
    [Export] private Sprite2D Sprite;
    private bool CanClick;

    private float ClickDelay;
    private float ClickTimer;
    private float HoldTimer;

    public override void _Ready()
    {
        ClickDelay = 1.0f / MaxCPS;
    }

    public override void _Process(double delta)
    {
        if (ClickTimer > 0)
        {
            ClickTimer -= (float)delta;
        }

        if (HoldTimer >= HoldThreshold)
        {
            ShopManager.SM.Show();
        }

        if (!BuildManager.BM.IsBuilding && CanClick && ClickTimer <= 0)
        {
            if (Input.IsActionJustPressed("Interact"))
            {
                ClickTimer = ClickDelay;
                GameManager.GM.Currency += GameManager.GM.Efficiency;
            }

            if (Input.IsActionPressed("Interact") )
            {
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
        Tween tween = CreateTween();
        tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1), 0.15f);
        CanClick = true;
    }

    private void OnMouseExited()
    {
        Tween tween = CreateTween();
        tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1, 0), 0.15f);
        CanClick = false;
    }

}
