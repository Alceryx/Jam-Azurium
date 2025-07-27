using Godot;
using System;
using Godot.Collections;

public partial class ShopManager : Control
{
    [ExportGroup("List")]
    [Export] public VBoxContainer ItemList;
    [Export] public VBoxContainer UpgradeList;
    
    [ExportGroup("Item")]
    [Export] public Array<TurretData> Database = [];
    [Export] public PackedScene ItemFrame;

    [ExportGroup("Tab")]
    [Export] public TextureButton DefenceTab;
    [Export] public TextureButton UpgradeTab;
    [Export] private PanelContainer defence;
    [Export] private PanelContainer upgrade;

    [ExportGroup("Button")]
    [Export] public TextureButton Buy;
    
    public static ShopManager SM;
    
    public bool AnySelected;
    public ShopItem SelectedItem;
    public Upgrade SelectedUpgrade;
    public ButtonGroup Mode = new();

    public Upgrade Active;

    public override void _Ready()
    {
        SM = this;
        Buy.Disabled = true;
        
        upgrade.Hide();
        
        Mode.AllowUnpress = true;
        
        ButtonGroup tab = new ButtonGroup();
        DefenceTab.SetButtonGroup(tab);
        UpgradeTab.SetButtonGroup(tab);
        
        DisplayItem();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Escape")) Hide();
        if (PriceCheck())
        {
            Buy.Disabled = !AnySelected;
        }
        else Buy.Disabled = true;
    }
    
    public void OnPurchase()
    {
        if (SelectedItem != null && GameManager.GM.Currency >= SelectedItem.Price)
        {
            LockItem();
            
            if (HotbarManager.HM.TurretsSlot.ContainsKey(SelectedItem.Data))
            {
                if (HotbarManager.HM.TurretsSlot[SelectedItem.Data].ContainsKey(SelectedItem.Mode))
                {
                    HotbarManager.HM.UpdateSlot(SelectedItem.Data, SelectedItem.Mode);
                }
                else HotbarManager.HM.AddSlot(SelectedItem.Data, SelectedItem.Mode);
            }
            else HotbarManager.HM.AddSlot(SelectedItem.Data, SelectedItem.Mode);
            
            GameManager.GM.Currency -= SelectedItem.Price;
        }
        else if (GameManager.GM.Currency >= SelectedUpgrade.Database[SelectedUpgrade.CurrentTier].Price)
        {
            LockUpgrade();
            
            SelectedUpgrade.Update();
            GameManager.GM.Currency -= SelectedUpgrade.Database[SelectedUpgrade.CurrentTier].Price;
        }
    }

    public void LockItem()
    {
        foreach (ShopItem item in ItemList.GetChildren())
        {
            if (item != SelectedItem)
            {
                item.Panel.Hide();
                item.Lock.Show();
            }
        }
    }

    public void LockUpgrade()
    {
        foreach (Upgrade upgrade in UpgradeList.GetChildren())
        {
            if (upgrade != SelectedUpgrade)
            {
                upgrade.Panel.Hide();
                upgrade.Lock.Show();
            }
        }
    }

    private bool PriceCheck()
        => defence.Visible && SelectedItem != null && GameManager.GM.Currency >= SelectedItem.Price
           || upgrade.Visible && SelectedUpgrade != null && GameManager.GM.Currency >= SelectedUpgrade.Database[SelectedUpgrade.CurrentTier].Price;
    private void OnClosePressed() => Hide();

    private void DisplayItem()
    {
        foreach (TurretData data in Database)
        {
            ShopItem item = ItemFrame.Instantiate() as ShopItem;
            item.SetUp(data, 0);
            ItemList.AddChild(item);
        }
    }
    private void PackUp()
    {
        foreach (Node item in ItemList.GetChildren())
        {
            item.QueueFree();
        }
    }
    
    public void OnDefenceToggled(bool toggled_on)
    {
        DisplayItem();
        defence.Visible = toggled_on;
        
        SelectedItem = null;
        SelectedUpgrade = null;
        AnySelected = false;
    }
    public void OnUpgradeToggled(bool toggled_on)
    {
        PackUp();
        upgrade.Visible = toggled_on;
        
        SelectedItem = null;
        SelectedUpgrade = null;
        AnySelected = false;
    }
}
