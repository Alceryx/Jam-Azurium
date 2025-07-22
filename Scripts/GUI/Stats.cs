using Godot;
using System;

public partial class Stats : Control
{
    [Export] private Label currency;

    public override void _PhysicsProcess(double delta)
    {
        currency.Text = GameManager.GM.Currency.ToString();
    }
}
