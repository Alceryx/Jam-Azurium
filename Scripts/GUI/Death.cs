using Godot;
using System;

public partial class Death : Control
{
    [Export] private Label WaveSurvived;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Hide();
    }

    public override void _Process(double delta)
    {
        if (GameManager.GM.ButtonHP <= 0)
        {
            GetTree().Paused = true;
            WaveSurvived.Text = $"{WaveManager.WM.CurrentWaveNumber - 1}";
            Show();
        }
    }

    public void OnReplayPressed()
    {
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
