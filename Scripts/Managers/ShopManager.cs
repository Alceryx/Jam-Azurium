using Godot;
using System;
using Godot.Collections;

public partial class ShopManager : Control
{
    [Export] public VBoxContainer ItemList;
    [Export] public PackedScene Item;
    [Export] public Array<TurretData> Database = [];
    [Export] public TextureButton Buy;

    [Export] public TextureButton DefenceTab;
    [Export] public TextureButton UpgradeTab;
    [Export] private PanelContainer defence;

    public static ShopManager SM;
    
    public bool AnySelected;
    public ShopItem SelectedItem;
    public ButtonGroup Mode = new();

    public override void _Ready()
    {
        SM = this;
        Buy.Disabled = true;
        
        Mode.AllowUnpress = true;
        
        ButtonGroup tab = new ButtonGroup();
        DefenceTab.SetButtonGroup(tab);
        UpgradeTab.SetButtonGroup(tab);

        foreach (TurretData data in Database)
        {
            ShopItem item = Item.Instantiate() as ShopItem;
            item.SetUp(data, 0);
            ItemList.AddChild(item);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("Escape")) Hide();
        Buy.Disabled = !AnySelected;
    }
    
    private void OnClosePressed() => Hide();

    public void OnPurchase()
    {
        if (GameManager.GM.Currency >= SelectedItem.Price)
        {
            Lock();
            
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
    }

    public void Lock()
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

    public void OnDefenceToggled(bool toggled_on)
    {
        defence.Visible = toggled_on;
    }
}
