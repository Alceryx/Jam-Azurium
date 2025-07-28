using Godot;
using System;

public partial class EfficiencyUpgrade : Upgrade
{
    public override void _Process(double delta)
    {
        base._Process(delta);
        Info.Text = Maxed ? "Maxed" :$"+{Database[CurrentTier].Addition} Efficiency";
    }
    
    public override void Update()
    {
        GameManager.GM.Efficiency += Database[CurrentTier].Addition;
        base.Update();
    }
}
