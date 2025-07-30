using Godot;
using System;

public partial class SceneManager : Node
{
    public static SceneManager SM;

    public Node CurrentScene;
    
    public override void _Ready()
    {
        SM = this;
        CurrentScene = GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1);
    }

    public void SwitchScene(string scene)
    {
        CallDeferred("DeferredSwitch", scene);
    }
    
    public void PreserveOnLoad(Node node)
    {
        node.Reparent(this);
    }
    private void DeferredSwitch(string scene)
    {
        CurrentScene.QueueFree();
        PackedScene NewScene = ResourceLoader.Load<PackedScene>(scene);
        CurrentScene = NewScene.Instantiate();
        GetTree().Root.AddChild(CurrentScene);
        GetTree().CurrentScene = CurrentScene;
    }
}

