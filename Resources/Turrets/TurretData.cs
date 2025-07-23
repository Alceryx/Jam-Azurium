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
    [Export] public int ID; //ID of the turret for spawning
    [Export] public string Name; //Name of the turret
    [Export] public Vector2 Size; //Size (in grid cells)
    [Export] public Texture2D Icon; //Icon (Preview icon)
    [Export] public int Price;
    [Export] public int ModeCount;

    //Todo: Add stats properties
}
#endif