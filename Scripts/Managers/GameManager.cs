using Godot;
using Godot.Collections;

using System;

public partial class GameManager : Node
{
    public static GameManager GM;

    public int Currency;
    public int Efficiency;

    public override void _Ready()
    {
        GM = this;
    }
 }
