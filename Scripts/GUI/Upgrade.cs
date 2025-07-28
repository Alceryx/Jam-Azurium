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
    [Export] private HBoxContainer CostBar;

    private Texture2D SelectHover;
    private Texture2D SelectPressed;
    
    [ExportGroup("Detail")]
    [Export] public Label Info;
    [Export] public Label Price;

    [ExportGroup("Dynamic")] 
    [Export] public PanelContainer Panel;
    [Export] public TextureRect Lock;

    [ExportGroup("Button")]
    [Export] private TextureButton Select;

    public int CurrentTier = 0;
    public bool Maxed => CurrentTier == Database.Count;
    public bool Affordable;
    protected bool IsSelected;

    public override void _Ready()
    {
        SelectHover = Select.TextureHover;
        SelectPressed = Select.TexturePressed;
        
        Select.Pressed += OnSelection;
        Lock.Hide();
    }

    public override void _Process(double delta)
    {
        if (!Maxed)
        {
            if (GameManager.GM != null && GameManager.GM.Currency < Database[CurrentTier].Price)
            {
                Price.AddThemeColorOverride("font_color", Color.FromString("A94241", Colors.White));
                Affordable = false;
            }
            else
            {
                Price.AddThemeColorOverride("font_color", Colors.White);
                Affordable = true;
            }
        }
        else
        {
            CostBar.Hide();
        }
        
        if (ShopManager.SM.ActiveUpgrade != this || ShopManager.SM.SelectedUpgrade == null) Deselect();

        Price.Text = Maxed ? string.Empty : $"{Database[CurrentTier].Price}";
    }

    public virtual void Update()
    {
        Tier.Texture = Database[CurrentTier].Graphic;
        if (CurrentTier < Database.Count) CurrentTier++;
    }

    private void OnSelection()
    {
        IsSelected = !IsSelected;
        Background.Texture = IsSelected ? BGSelected : BGDefault;
        switch (IsSelected)
        {
            case true:
                DisableHoverGraphic();
                break;
            case false:
                EnableHoverGraphic();
                break;
        }

        ShopManager.SM.SelectedUpgrade = IsSelected ? this : null;
        ShopManager.SM.AnySelected = IsSelected;
        ShopManager.SM.ActiveUpgrade = this;
    }

    private void DisableHoverGraphic()
    {
        Select.TextureHover = null;
        Select.TexturePressed = null;
    }
    private void EnableHoverGraphic()
    {
        Select.TextureHover = SelectHover;
        Select.TexturePressed = SelectPressed;
    }
    
    public void Deselect()
    {
        IsSelected = false;
        Background.Texture = BGDefault;
    }
}
