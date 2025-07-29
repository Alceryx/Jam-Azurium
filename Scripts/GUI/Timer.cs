using Godot;
using System;

public partial class Timer : Control
{
    [Export] private TextureRect Background;
    [Export] private Texture2D TimeExpiring;
    [Export] private Label time;
    [Export] private Label wave;

    private Texture2D BGDefault;

    public override void _Ready()
    {
        BGDefault = Background.Texture;
    }

    public override void _Process(double delta)
    {
        string minute = Mathf.Floor(WaveManager.WM.WaveTimer / 60) >= 10 ? Mathf.Floor(WaveManager.WM.WaveTimer / 60).ToString() : "0" + Mathf.Floor(WaveManager.WM.WaveTimer / 60); 
        string second = Mathf.Floor(WaveManager.WM.WaveTimer % 60) >= 10 ? Mathf.Floor(WaveManager.WM.WaveTimer % 60).ToString() : "0" + Mathf.Floor(WaveManager.WM.WaveTimer % 60); 
        time.Text = minute + ":" + second;
        wave.Text = $"{WaveManager.WM.CurrentWaveNumber}/{WaveManager.WM.Waves.Count}";

        if (WaveManager.WM.WaveTimer <= 5) EndSoon();
        else Default();
    }

    private void Default()
    {
        Background.Texture = BGDefault;
        time.AddThemeColorOverride("font_color", Color.FromString("73DEE2", Colors.White));
        wave.AddThemeColorOverride("font_color", Color.FromString("2b4f6f", Colors.White));
    }

    private void EndSoon()
    {
        Background.Texture = TimeExpiring;
        time.AddThemeColorOverride("font_color", Color.FromString("D22424", Colors.White));
        wave.AddThemeColorOverride("font_color", Color.FromString("7A0000", Colors.White));
    }
}
