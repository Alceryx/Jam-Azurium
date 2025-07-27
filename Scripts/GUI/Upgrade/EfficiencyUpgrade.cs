using Godot;
using System;

public partial class EfficiencyUpgrade : Upgrade
{
    public override void _Process(double delta)
    {
        base._Process(delta);
        Info.Text = $"+{Database[CurrentTier].Addition} Efficiency";
    }
    
    public override void Update()
    {
        base.Update();
        GameManager.GM.Efficiency += Database[CurrentTier].Addition;
    }
}
