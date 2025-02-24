using System.Collections.Generic;
using Godot;

public partial class Teleporter : Building
{
    public int frequency {get; private set;} = 0;
    List<Teleporter> teleporters;

    Teleporter teleportOutput = null;
    bool isConnected;

    Button button;

    public Teleporter(Vector2I pos, InternalBeltCreatedEventHandler outputCreated, List<Teleporter> teleporters) : base(pos, "Teleporter", outputCreated)
    {
        mode = PlaceMode.Teleporter;
        this.teleporters = teleporters;
        AddOutput(Vector2I.Zero);

        area.AreaEntered += AreaEntered;

        button = new Button(){
            Size = Vector2.One * 60,
            Text = "0",
            Position = Vector2.One * -36,
            MouseFilter = Control.MouseFilterEnum.Pass
            //add theme
        };
        AddChild(button);
        button.Pressed += ButtonPressed;
    }

    public void SetTeleportInput(Teleporter teleporter)
    {
        teleportOutput = teleporter;
        isConnected = true;

        output[0].Connect(BeltInput.Center);
        sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/TeleporterInput.png");
    }

    public void UnsetTeleportInput()
    {
        if (teleportOutput != null)
        {
            teleportOutput = null;
            isConnected = false;
            output[0].OutputLost();
            sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/Teleporter.png");
        }
    }

    public void TeleportItem(Item item)
    {
        item.Position = pos * Map.tilesize;
        item.belt = output[0];
    }

    private void AreaEntered(Area2D other)
    {
        if (other.Owner is Item item)
        {
            if (teleportOutput != null)
                teleportOutput.TeleportItem(item);
        }
    }

    private void ButtonPressed()
    {
        frequency = frequency >= 9 ? 0 : frequency + 1;
        button.Text = frequency.ToString();

        UnsetTeleportInput();
        foreach(Teleporter teleporter in teleporters)
        {
            if (teleporter.frequency == frequency && teleporter.isConnected)
            {
                if (teleporter.teleportOutput != null)
                    SetTeleportInput(teleporter.teleportOutput);
                else
                    SetTeleportInput(teleporter);
                return;
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isConnected)
        {
            if (teleportOutput == null && output[0].output == BeltInput.None)
            {
                isConnected = false;
                button.Disabled = false;
                sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/Teleporter.png");
                foreach(Teleporter teleporter in teleporters)
                {
                    if (teleporter.frequency == frequency && teleporter.isConnected)
                        teleporter.UnsetTeleportInput();
                }
            }
            return;
        }
        if (output[0].output != BeltInput.None)
        {
            isConnected = true;
            button.Disabled = true;
            sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/TeleporterOutput.png");

            foreach(Teleporter teleporter in teleporters)
            {
                if (teleporter == this)
                    continue;
                if (teleporter.frequency == frequency)
                    teleporter.SetTeleportInput(this);
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    public override void Remove()
    {
        teleporters.Remove(this);

        if (!isConnected || teleportOutput != null)
            return;
        
        foreach(Teleporter teleporter in teleporters)
        {
            if (teleporter.frequency == frequency && teleporter.isConnected)
                teleporter.UnsetTeleportInput();
        }
    }
}