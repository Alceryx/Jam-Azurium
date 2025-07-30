using Godot;
using System;

public partial class Pause : Control
{
    [Export] private TextureButton Fullscreen;
    
    public override void _Ready()
    {
        GetTree().Paused = true;
        ProcessMode = ProcessModeEnum.Always;
        Fullscreen.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Pause"))
        {
            OnContinuePressed();
        }
    }

    private void OnContinuePressed()
    {
        GetTree().Paused = false;
        QueueFree();
    }
    
    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
    
    private void OnButtonToggled(bool toggled_on)
    {
        if (toggled_on)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else if (!toggled_on)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
    }
}
