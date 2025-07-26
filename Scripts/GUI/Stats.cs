using Godot;
using System;

public partial class Stats : Control
{
    [Export] private Label currency;
    [Export] private TextureProgressBar health;
    [Export] private RichTextLabel efficiency;

    public override void _PhysicsProcess(double delta)
    {
        currency.Text = $"{GameManager.GM.Currency}";
        health.Value = GameManager.GM.ButtonHP;
        efficiency.Text = $"{GameManager.GM.Efficiency}/click";
    }
}
