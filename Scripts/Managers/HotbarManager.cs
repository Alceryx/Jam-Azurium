using Godot;
using Godot.Collections;
using System;

public partial class HotbarManager : Control
{
    [Export] public HBoxContainer Bar;
    [Export] public PackedScene Slot;

    public static HotbarManager HM;
    
    public override void _Ready()
    {
        HM = this;
    }

    public void AddSlot(int amount)
    {
        if (Bar.GetChildCount() > 0) return;
        
        for (int i = 0; i < amount; i++)
        {
            HotbarSlot slot = Slot.Instantiate() as HotbarSlot;
            Bar.AddChild(slot);
        }
    }

    public void ItemToBar(int mode, ShopItem item)
    {
        Bar.GetChild<HotbarSlot>(mode).SetUp(item.Icon.Texture, item.Amount);
    }
}
