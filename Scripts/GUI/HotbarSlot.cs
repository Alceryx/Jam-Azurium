using Godot;
using System;
using Godot.Collections;

public partial class HotbarSlot : TextureButton
{
    [ExportGroup("Graphic")] 
    [Export] private Array<Texture2D> Mode1Icon = [];
    [Export] private Array<Texture2D> Mode2Icon = [];

    [ExportGroup("Details")]
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
        SetGraphic(mode);
        
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

    private void SetGraphic(int mode)
    {
        switch (mode)
        {
            case 1:
                TextureNormal = Mode1Icon[0];
                TextureHover = Mode1Icon[1];
                TexturePressed = Mode1Icon[2];
                break;
            case 2:
                TextureNormal = Mode2Icon[0];
                TextureHover = Mode2Icon[1];
                TexturePressed = Mode2Icon[2];
                break;
        }
    }

    private void OnPlacementFinished(TurretData data, int mode)
    {
        if (data == Data && mode == Mode)
        {
            Amount -= 1;
            if (Amount == 0)
            {
                HotbarManager.HM.TurretsSlot[data].Remove(mode);
                if (HotbarManager.HM.TurretsSlot[data].Count == 0)
                {
                    HotbarManager.HM.TurretsSlot.Remove(data);
                    if (HotbarManager.HM.TurretsSlot.Count == 0)
                        HotbarManager.HM.Hide();
                }
                                
                QueueFree();
            }
        }
    }
}
