using Godot;
using Godot.Collections;
using System;

public partial class ShopItem : PanelContainer
{
    [ExportGroup("Dynamic")]
    [Export] public PanelContainer Panel;
    [Export] public TextureRect Lock;
    [Export] public TextureRect Icon;
    [Export] public Array<TextureButton> ModeButton = [];
    
    [ExportGroup("Detail")]
    [Export] private Label name;
    [Export] private Label price;
    [Export] private RichTextLabel description;

    public bool Affordable;
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
        if (!ShopManager.SM.Defence.Visible) Deselect();
        if (GameManager.GM != null && GameManager.GM.Currency < Data.Modes[Mode].Price)
        {
            price.AddThemeColorOverride("font_color", Color.FromString("A94241", Colors.White));
            Affordable = false;
        }
        else
        {
            price.AddThemeColorOverride("font_color", Colors.White);
            Affordable = true;
        }
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

    public void Deselect()
    {
        foreach (TextureButton button in ModeButton)
        {
            button.ButtonPressed = false;
        }
    }

    private void OnMode1Selected(bool toggled_on)
    {
        SetUp(Data, 0);
        
        ShopManager.SM.SelectedItem = toggled_on ? this : null;
        ShopManager.SM.AnySelected = toggled_on;
    }
    private void OnMode2Selected(bool toggled_on)
    {
        SetUp(Data, 1);
        
        ShopManager.SM.SelectedItem = toggled_on ? this : null;
        ShopManager.SM.AnySelected = toggled_on;
    }
}
