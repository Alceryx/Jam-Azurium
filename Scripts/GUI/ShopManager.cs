using Godot;
using System;
using Godot.Collections;

public partial class ShopManager : Control
{
    [Export] public VBoxContainer ItemList;
    [Export] public PackedScene Item;
    [Export] public Array<TurretData> Database = [];

    public override void _Ready()
    {
        AddItem();
    }

    private void AddItem()
    {
        foreach (TurretData data in Database)
        {
            ShopItems item = Item.Instantiate() as ShopItems;
            item.SetUp(data);
            ItemList.AddChild(item);
        }
    }
}
