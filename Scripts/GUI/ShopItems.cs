using Godot;
using Godot.Collections;
using System;

public partial class ShopItems : PanelContainer
{
    [Export] private Label name;
    [Export] private TextureRect icon;
    [Export] private Label price;

    private TurretData data;
    
    public void SetUp(TurretData data, int mode)
    {
        this.data = data;
        
        name.Text = data.Name;
        icon.Texture = data.Modes[mode].Icon;
        price.Text = data.Modes[mode].Price.ToString();
    }

    public void OnMode1Selected() => SetUp(data, 0);
    public void OnMode2Selected() => SetUp(data, 1);
}
