using Godot;
using System;

public partial class HotbarSlot : Button
{
    [Export] private TextureRect icon;
    [Export] private Label amount;

    public int Amount;
    public TurretData Data;
    public int Mode;

    public override void _Ready()
    {
        BuildManager.BM.PlacedTurret += (data, mode) => OnPlacementFinished(data, mode);
    }

    public override void _Process(double delta)
    {
        amount.Text = $"x{Amount}";
    }

    public void SetUp(TurretData data, int mode, int amount)
    {
        Amount = amount;
        Data = data;
        Mode = mode;
        icon.Texture = data.Modes[mode].Icon;
    }

    private void OnButtonPressed()
    {
        BuildManager.BM.SetTurret(Data, Mode);
        BuildManager.BM.Build();
    }

    private void OnPlacementFinished(TurretData data, int mode)
    {
        if (data == Data && mode == Mode)
        {
            Amount -= 1;
            if (Amount == 0)
            {
                HotbarManager.HM.TurretsSlot[data].Remove(mode);
                QueueFree();
            }
        }
    }
}
