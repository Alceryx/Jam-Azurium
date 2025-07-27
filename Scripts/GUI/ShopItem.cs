using Godot;
using Godot.Collections;
using System;

public partial class ShopItem : PanelContainer
{
    [Export] public TextureRect Lock;
    [Export] public TextureRect Icon;
    [Export] public PanelContainer Panel;
    [Export] public Array<TextureButton> ModeButton = [];
    
    [Export] private Label name;
    [Export] private Label price;
    
    public int Price;

    public int ModeCount;
    public int Mode;
    public TurretData Data;

    public override void _Ready()
    {
        Lock.Hide();
        foreach (TextureButton button in ModeButton) button.SetButtonGroup(ShopManager.SM.Mode);
    }

    public override void _Process(double delta)
    {
        if (GameManager.GM != null && GameManager.GM.Currency < Data.Modes[Mode].Price)
            price.AddThemeColorOverride("font_color", Colors.Red);
        else
            price.AddThemeColorOverride("font_color", Colors.White);
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
