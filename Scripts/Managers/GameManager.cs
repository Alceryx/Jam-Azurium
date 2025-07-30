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

    [ExportGroup("Pause")] 
    [Export] private PackedScene PauseMenu;
    
    public int Currency;
    public NexusButton NexusButton;
    

    public override void _Ready()
    {
        GM = this;
        ButtonHP = ButtonMaxHP;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Escape"))
        {
            Pause pause = PauseMenu.Instantiate<Pause>();
            GetTree().Root.AddChild(pause);
        }
    }
}
