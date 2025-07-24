using Godot;
using Godot.Collections;
using System;

public partial class ShopItems : PanelContainer
{
    [Export] private Label name;
    [Export] private TextureRect icon;
    [Export] private Label price;
    
    public void SetUp(TurretData data)
    {
        name.Text = data.Name;
        icon.Texture = data.Modes[0].Icon;
        price.Text = data.Price.ToString();
    }
}
