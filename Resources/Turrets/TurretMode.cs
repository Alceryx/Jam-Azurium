using Godot;
using System;

[Tool]
[GlobalClass]
#if TOOLS
public partial class TurretMode : Resource
{
    [ExportGroup("Basic Info")]
    [Export] public Texture2D Icon; //Icon (Preview icon)
    [Export] public int Price;
    [Export] public bool CanRotate; //Can Rotate on placing
    
    [ExportGroup("Stats")] 
    [Export] public PackedScene Projectile;
    [Export] public float Damage;
    [Export(PropertyHint.Range, "0, 100, 1, or_greater, suffix:tiles")] public float Range;
}
#endif
