using Godot;
using System;
using Godot.Collections;

public partial class ShopManager : Control
{
    [Export] public VBoxContainer ItemList;
    [Export] public PackedScene Item;
    [Export] public Array<TurretData> Database = [];

    public bool IsOpened;

    public override void _Ready()
    {
        foreach (TurretData data in Database)
        {
            ShopItems item = Item.Instantiate() as ShopItems;
            item.SetUp(data, 0);
            ItemList.AddChild(item);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("Escape")) Hide();
    }
    
    private void OnClosePressed() => Hide();

    public void LockItem()
    {
        foreach (ShopItems item in ItemList.GetChildren())
        {
            if (item.IsPurchased) continue;
            item.SetProcessInput(false);
        }
    }
}
