using Godot;
using System;

public partial class HealthUpgrade : Upgrade
{
    public override void _Process(double delta)
    {
        base._Process(delta);
        Info.Text = $"+{Database[CurrentTier].Addition} Max HP";
    }

    public override void Update()
    {
        base.Update();
        GameManager.GM.ButtonHP += Database[CurrentTier].Addition;
        GameManager.GM.ButtonMaxHP += Database[CurrentTier].Addition;
    }
}
