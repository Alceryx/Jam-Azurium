using Godot;
using System;
using Godot.Collections;

[Tool]
[GlobalClass]
#if TOOLS
public partial class SpawnData : Resource
{
    /*
     * Sequential meaning that this batch will spawn all of its load before moving onto the next batch
     * Parallel meaning that this batch will spawn parallel to all of the batches after it that has mode = parallel (until it reaches one that is sequential)
     * For example: if an array of spawn data looks like this (s: sequential, p: parallel)
     * [s, p, p, p, s, p, p].
     * The first batch is sequential so it'll spawn all of its enemies first then move onto the next
     * The next 3 batches are parallel so they will spawn together
     * The fourth one is sequential so it'll spawn all of its enemies first then move onto the next
     * The last 2 batches are parallel and will spawn together
     */
    public enum SpawnMode
    {
        Sequential,
        Parallel,
    }
    
    [Export] public PackedScene Enemy;
    [Export] public int Count;
    [Export] public float SpawnInterval; //The Interval between each enemy spawned
    [Export] public SpawnMode Mode;
}
#endif
