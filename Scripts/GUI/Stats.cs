using Godot;
using System;

public partial class Stats : Control
{
    [Export] private Label health;
    [Export] private TextureProgressBar healthBar;
    [Export] private RichTextLabel efficiency;
    [Export] private Label currency;

    public override void _PhysicsProcess(double delta)
    {
        healthBar.Value = GameManager.GM.ButtonHP;
        healthBar.MaxValue = GameManager.GM.ButtonMaxHP;

        health.Text = $"{GameManager.GM.ButtonHP}/{GameManager.GM.ButtonMaxHP}";
        currency.Text = $"{GameManager.GM.Currency}";
        efficiency.Text = $"{GameManager.GM.Efficiency} / click";
    }
}
