using Godot;
using System;

public partial class NexusButton : StaticBody2D
{
    [Export] private int MaxCPS;
    [Export] private Sprite2D Sprite;
    private bool CanClick;

    private float ClickDelay;
    private float ClickTimer;

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
        
        if (!BuildManager.BM.IsBuilding && Input.IsActionJustPressed("Interact") && CanClick && ClickTimer <= 0)
        {
            ClickTimer = ClickDelay;
            GameManager.GM.Currency += 1;
            GD.Print(GameManager.GM.Currency);
        }
    }
    private void OnMouseEntered()
    {
        Tween tween = CreateTween();
        tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1), 0.15f);
        tween.Parallel().TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.15f);
        CanClick = true;
    }

    private void OnMouseExited()
    {
        Tween tween = CreateTween();
        tween.TweenProperty((ShaderMaterial)Sprite.Material, "shader_parameter/outline_color", new Color(1,1,1, 0), 0.15f);
        tween.Parallel().TweenProperty(this, "scale", new Vector2(1, 1), 0.15f);
        CanClick = false;
    }
}
