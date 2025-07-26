using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class ShopManager : Control
{
    [Export] public VBoxContainer ItemList;
    [Export] public PackedScene Item;
    [Export] public Array<TurretData> Database = [];
    [Export] public Button Buy;

    public static ShopManager SM;
    
    public bool AnySelected;
    public ShopItem SelectedItem;

    public override void _Ready()
    {
        SM = this;
        Buy.Disabled = true;
        
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
        if (GameManager.GM.Currency > SelectedItem.Price)
        {
            Lock();
            if (!SelectedItem.Amount.TryAdd(SelectedItem.Mode, 1)) SelectedItem.Amount[SelectedItem.Mode]++;

            HotbarManager.HM.AddSlot(SelectedItem.ModeCount);
            HotbarManager.HM.ItemToBar(SelectedItem.Mode, SelectedItem);
            GameManager.GM.Currency -= SelectedItem.Price;
        }
    }

    public void Lock()
    {
        foreach (ShopItem item in ItemList.GetChildren())
        {
            if (item != SelectedItem) item.Lock.Show();
        }
    }
}
