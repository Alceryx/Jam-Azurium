using Godot;
using System;

[Tool]
[GlobalClass]
#if TOOLS
public partial class TurretMode : Resource
{
    [ExportGroup("Basic Info")]
    [Export] public Texture2D Icon; //Icon (Preview icon)

    [ExportGroup("Stats")] 
    [Export] public PackedScene Projectile;
    [Export] public float Damage;
    [Export] public float BulletPerShot;
    [Export(PropertyHint.Range, "0, 360, radians_as_degrees")] public float Arc;
}
#endif
