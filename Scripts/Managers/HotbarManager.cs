using Godot;
using Godot.Collections;
using System;

public partial class HotbarManager : Control
{
    [Export] public HBoxContainer Bar;
    [Export] public PackedScene Slot;

    public Dictionary<TurretData, Dictionary<int, HotbarSlot>> TurretsSlot = new();
    
    public static HotbarManager HM;
    
    public override void _Ready()
    {
        HM = this;
    }

    public void AddSlot(TurretData data, int mode)
    {
        HotbarSlot slot = Slot.Instantiate() as HotbarSlot;
        
        if (!Visible) Show();
        
        if (TurretsSlot.ContainsKey(data)) TurretsSlot[data].Add(mode, slot);
        else TurretsSlot.Add(data, new Dictionary<int, HotbarSlot> {{mode, slot}});
        slot.SetUp(data, mode, 1);
        Bar.AddChild(slot);
    }

    public void UpdateSlot(TurretData data, int mode)
    {
        TurretsSlot[data][mode].SetUp(data, mode, TurretsSlot[data][mode].Amount + 1);
    }
}
