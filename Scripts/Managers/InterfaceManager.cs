using Godot;
using System;

public partial class InterfaceManager : CanvasLayer
{
    [Export] public ShopManager ShopMenu;

    public static InterfaceManager IM;

    public override void _Ready()
    {
        IM = this;
    }
}
