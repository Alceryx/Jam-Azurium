using Godot;
using Godot.Collections;
using System;

public partial class Upgrade : PanelContainer
{
    [ExportGroup("Data")]
    [Export] public Array<UpgradeData> Database = [];
    
    [ExportGroup("Graphic")]
    [Export] private TextureRect Background;
    [Export] public Texture2D BGDefault;
    [Export] public Texture2D BGSelected;
    [Export] private TextureRect Tier;
    
    [ExportGroup("Detail")]
    [Export] public Label Info;
    [Export] public Label Price;

    [ExportGroup("Dynamic")] 
    [Export] public PanelContainer Panel;
    [Export] public TextureRect Lock;

    [ExportGroup("Button")]
    [Export] private TextureButton Select;

    public int CurrentTier = 0;
    public bool IsSelected;

    public override void _Ready()
    {
        Select.Pressed += OnSelection;
        Lock.Hide();
    }

    public override void _Process(double delta)
    {
        Price.Text = $"{Database[CurrentTier].Price}";
        if (ShopManager.SM.Active != this || ShopManager.SM.SelectedUpgrade == null) Deselect();
    }

    public virtual void Update()
    {
        CurrentTier++;
        Tier.Texture = Database[CurrentTier].Graphic;
    }

    private void OnSelection()
    {
        IsSelected = !IsSelected;
        Background.Texture = IsSelected ? BGSelected : BGDefault;

        ShopManager.SM.SelectedUpgrade = IsSelected ? this : null;
        ShopManager.SM.AnySelected = IsSelected;
        ShopManager.SM.Active = this;
    }

    public void Deselect()
    {
        IsSelected = false;
        Background.Texture = BGDefault;
    }
}
