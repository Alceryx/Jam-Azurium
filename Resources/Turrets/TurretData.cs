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
    [Export] public PackedScene Turret;
    [Export] public string Name;
    [Export] public Vector2 Size;
    [Export] public Texture Icon;
    
    //Todo: Add stats properties
}
#endif