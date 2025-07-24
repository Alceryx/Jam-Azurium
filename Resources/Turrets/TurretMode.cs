using Godot;
using System;

[Tool]
[GlobalClass]
#if TOOLS
public partial class TurretMode : Resource
{
    [ExportGroup("Basic Info")]
    [Export] public Texture2D Icon; //Icon (Preview icon)
    [Export] public bool CanRotate;
    
    [ExportGroup("Stats")] 
    [Export] public PackedScene Projectile;
    [Export] public float Damage;

    [ExportGroup("Rotation")] 
    [Export] public Texture2D TopLeft;
    [Export] public Texture2D TopRight;
    [Export] public Texture2D BottomLeft;
    [Export] public Texture2D BottomRight;
}
#endif
