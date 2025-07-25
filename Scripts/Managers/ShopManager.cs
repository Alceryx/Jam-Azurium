using Godot;
using System;
using Godot.Collections;

public partial class ShopManager : Control
{
    [Export] public VBoxContainer ItemList;
    [Export] public PackedScene Item;
    [Export] public Array<TurretData> Database = [];

    public bool IsOpened;

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("Escape")) Close();
    }

    public void Open()
    {
        if (IsOpened)
            return;
        
        Show();
        IsOpened = true;
        
        foreach (TurretData data in Database)
        {
            ShopItems item = Item.Instantiate() as ShopItems;
            item.SetUp(data);
            ItemList.AddChild(item);
        }
    }

    public void Close()
    {
        if (!IsOpened)
            return;
        
        IsOpened = false;
        Hide();
        
        foreach (var child in ItemList.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void OnClosePressed() => Close();
}
