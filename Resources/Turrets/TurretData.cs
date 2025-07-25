using Godot;
using Godot.Collections;
using System;

/*
 * Contains basic info about the turrets
 */
[Tool]
[GlobalClass]
#if TOOLS
public partial class TurretData : Resource
{
    [Export] public int ID; //ID of the turret for spawning
    [Export] public string Name; //Name of the turret
    [Export] public Vector2 Size; //Size (in grid cells)
    [Export] public Array<TurretMode> Modes;
}
#endif