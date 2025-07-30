using Godot;
using System;

public partial class Recess : Control
{
    [Export] private Label CountDown;

    public override void _Ready()
    {
        Hide();
    }

    public override void _Process(double delta)
    {
        if (!WaveManager.WM.CountDownFinished)
        {
            Show();
            if (WaveManager.WM.CountDownTimer <= 1) CountDown.Text = $"Wave {WaveManager.WM.CurrentWaveNumber}";
            else CountDown.Text = $"{(int)WaveManager.WM.CountDownTimer}";
        }
        else Hide();
    }
}
