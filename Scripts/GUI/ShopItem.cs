using Godot;
using Godot.Collections;
using System;
using Array = System.Array;

public partial class ShopItem : PanelContainer
{
    [Export] public Panel Lock;
    [Export] public TextureRect Icon;

    [Export] private Array<TextureButton> ModeButton = [];
    [Export] private Label name;
    [Export] private Label price;
    
    public int Price;

    public int ModeCount;
    public int Mode;
    public TurretData Data;

    public override void _Ready()
    {
        ButtonGroup grp = new ButtonGroup();
        grp.AllowUnpress = true;
        foreach (TextureButton button in ModeButton) button.SetButtonGroup(grp);
    }

    public void SetUp(TurretData data, int mode)
    {
        Price = data.Modes[mode].Price;

        ModeCount = data.Modes.Count;
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
