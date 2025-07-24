using Godot;
using Godot.Collections;
using System;

[Tool]
[GlobalClass]
#if TOOLS
public partial class WaveData : Resource
{
    [Export] public int WaveNumber;
    [Export] public Array<SpawnData> SpawnedEnemy;
}
#endif
