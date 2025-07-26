using Godot;
using Godot.Collections;
using System;

public partial class ShopItem : PanelContainer
{
    [Export] public Panel Lock;
    
    [Export] public TextureRect Icon;
    [Export] private Label name;
    [Export] private Label price;
    
    public int Price;
    public int Amount;

    public int ModeAmt;
    public int Mode;
    public TurretData Data;

    public void SetUp(TurretData data, int mode)
    {
        Price = data.Modes[mode].Price;

        ModeAmt = data.Modes.Count;
        Mode = mode;
        Data = data;
        
        name.Text = data.Name;
        Icon.Texture = data.Modes[mode].Icon;
        price.Text = $"{data.Modes[mode].Price}";
    }

    public void OnMode1Selected(bool toggled_on)
    {
        SetUp(Data, 0);

        ShopManager.SM.SelectedItem = toggled_on ? this : null;
        ShopManager.SM.AnySelected = toggled_on;
    }
    public void OnMode2Selected(bool toggled_on)
    {
        SetUp(Data, 1);
        
        ShopManager.SM.SelectedItem = toggled_on ? this : null;
        ShopManager.SM.AnySelected = toggled_on;
    }
}
