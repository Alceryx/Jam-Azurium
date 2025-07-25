using Godot;
using System;

public partial class Stats : Control
{
    [Export] private Label currency;
    [Export] private RichTextLabel efficiency;

    public override void _PhysicsProcess(double delta)
    {
        currency.Text = $"{GameManager.GM.Currency}";
        efficiency.Text = $"{GameManager.GM.Efficiency}/click";
    }
}
