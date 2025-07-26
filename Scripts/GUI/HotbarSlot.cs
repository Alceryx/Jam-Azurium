using Godot;
using System;

public partial class HotbarSlot : PanelContainer
{
    [Export] private TextureRect icon;
    [Export] private Label amount;

    public int Amount;

    public override void _Process(double delta)
    {
        amount.Text = $"x{Amount}";
    }

    public void SetUp(Texture2D icon, int amount)
    {
        Amount = amount;
        this.icon.Texture = icon;
    }
}
