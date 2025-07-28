using Godot;
using System;

[Tool]
[GlobalClass]
#if TOOLS
public partial class UpgradeData : Resource
{
    [Export] public int Price;
    [Export] public int Addition;
    [Export] public Texture2D Graphic;
}
#endif
