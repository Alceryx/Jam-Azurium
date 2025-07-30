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
    [Export] public PanelContainer Defence;
    [Export] public PanelContainer Upgrade;

    [ExportGroup("Button")]
    [Export] public TextureButton Buy;
    
    public static ShopManager SM;
    
    public bool AnySelected;
    public ShopItem SelectedItem;
    public Upgrade SelectedUpgrade;
    public ButtonGroup Mode = new();

    public Upgrade ActiveUpgrade;

    private bool SignalConnected;
    private bool IsInArea;

    public override void _Ready()
    {
        SM = this;
        Buy.Disabled = true;
        
        Upgrade.Hide();
        
        Mode.AllowUnpress = true;
        
        ButtonGroup tab = new ButtonGroup();
        DefenceTab.SetButtonGroup(tab);
        UpgradeTab.SetButtonGroup(tab);
        
        DisplayItem();
    }

    public override void _Process(double delta)
    {
        if (IsInArea)
        {
            GameManager.GM.NexusButton.CanClick = false;
        }
        
        if (!SignalConnected)
        {
            WaveManager.WM.WaveEnded += Unlock;
            WaveManager.WM.WaveStarted += Hide;
            SignalConnected = true;
        }

        if (Input.IsActionJustPressed("Escape"))
        {
            Hide();
        }

        if (Affordable()) Buy.Disabled = !AnySelected;
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
            
            GameManager.GM.Currency -= SelectedUpgrade.Database[SelectedUpgrade.CurrentTier].Price;
            SelectedUpgrade.Update();
        }
    }
    
    public bool Affordable()
    {
        return SelectedItem is {Affordable: true } || SelectedUpgrade is {Affordable: true, Maxed: false };
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

    public void Unlock()
    {
        foreach (Upgrade upgrade in UpgradeList.GetChildren())
        {
            upgrade.Panel.Show();
            upgrade.Lock.Hide();
        }
        
        PackUp();
        DisplayItem();
    }

    private void OnClosePressed()
    {
        Hide();
    }
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
    
    private void OnDefenceToggled(bool toggled_on)
    {
        Defence.Visible = toggled_on;
        
        SelectedItem = null;
        SelectedUpgrade = null;
        AnySelected = false;
    }
    private void OnUpgradeToggled(bool toggled_on)
    {
        Upgrade.Visible = toggled_on;
        
        SelectedItem = null;
        SelectedUpgrade = null;
        AnySelected = false;
    }

    private void OnMouseEntered()
    {
        IsInArea = true;
    }

    private void OnMouseExited()
    {
        IsInArea = false;
    }
}
