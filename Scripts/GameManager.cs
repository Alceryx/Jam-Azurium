using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager GM;

    public override void _Ready()
    {
        GM = this;
    }
}
