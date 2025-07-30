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

    [ExportGroup("Misc")] 
    [Export] private AudioStreamPlayer Audio;
    [Export] private float AudioDelay;
    [Export] private PackedScene PauseMenu;
    
    private float AudioDelayTimer;
    
    public int Currency;
    public NexusButton NexusButton;
    

    public override void _Ready()
    {
        GM = this;
        ButtonHP = ButtonMaxHP;
        AudioDelayTimer = AudioDelay;
    }

    public override void _Process(double delta)
    {
        if (AudioDelayTimer > 0)
        {
            AudioDelayTimer -= (float)delta;
            if (AudioDelayTimer <= 0)
                Audio.Play();
        } 
        
        if (Input.IsActionJustPressed("Escape") && !ShopManager.SM.Visible && !BuildManager.BM.IsBuilding)
        {
            Pause pause = PauseMenu.Instantiate<Pause>();
            GetTree().Root.AddChild(pause);
        }
    }
}
