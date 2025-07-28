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
        time.Text = WaveManager.WM.WaveTimer <= 0 ? "00.00" : $"{WaveManager.WM.WaveTimer:00.00}";
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
