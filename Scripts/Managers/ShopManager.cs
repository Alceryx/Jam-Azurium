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

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("Escape")) Hide();
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

    public void OnClosePressed() => Hide();
}
