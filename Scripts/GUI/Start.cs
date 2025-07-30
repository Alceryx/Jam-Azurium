using Godot;
using System;

public partial class Start : Control
{
    void OnStartPressed()
    {
        SceneManager.SM.SwitchScene("res://Scenes/World.tscn");
    }
}
