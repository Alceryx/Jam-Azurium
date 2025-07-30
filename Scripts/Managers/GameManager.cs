using Godot;
using Godot.Collections;

using System;

public partial class GameManager : Node
{
    public static GameManager GM;
    
    [ExportGroup("Nexus Button")]
    [Export] public int Efficiency;
    [Export] public float ButtonMaxHP;
    public float ButtonHP;
    
    public int Currency;
    public NexusButton NexusButton;
    

    public override void _Ready()
    {
        GM = this;
        ButtonHP = ButtonMaxHP;
    }
}
