using Godot;
using Godot.Collections;
using System;

public partial class ShopItems : TextureButton
{
    [Export] private Label name;
    [Export] private TextureRect icon;
    [Export] private Label price;

    public bool IsSelected;
    public bool IsPurchased;
    
    private TurretData data;
    private int mode;

    public override void _Ready()
    {
        SetProcessInput(false);
        foreach (var child in GetChildren())
        {
            SetProcessInput(false);
        }
    }
    
    public void OnMode1Selected() => SetUp(data, 0);
    public void OnMode2Selected() => SetUp(data, 1);

    public void SetUp(TurretData data, int mode)
    {
        this.data = data;
        this.mode = mode;
        
        name.Text = data.Name;
        icon.Texture = data.Modes[mode].Icon;
        price.Text = data.Modes[mode].Price.ToString();
    }

}
