using Godot;
using System;

/*
 * Contains basic info about the turrets
 */
[Tool]
[GlobalClass]
#if TOOLS
public partial class TurretData : Resource
{
    [Export] public PackedScene Turret; //Scene of the turret
    [Export] public string Name; //Name of the turret
    [Export] public Vector2 Size; //Size (in grid cells)
    [Export] public Texture Icon; //Icon (Preview icon)
    
    //Todo: Add stats properties
}
#endif