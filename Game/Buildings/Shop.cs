using Godot;

public partial class Shop : Building
{
    Timer timer;
    ItemType type;
    public Shop(Vector2I pos, ItemType type, int rot) : base(pos, type.ToString() + "Shop", false)
    {
        // newPosition.Y -= ;
		Position = Position - Vector2.Down * Map.tilesize/2;
        this.type = type;
        timer = new Timer(){
            Autostart = true,
            WaitTime = 20,
            OneShot = true,
        };
        timer.Timeout += Timeout;
        AddChild(timer);

        area.AreaEntered += AreaEntered;

        RotationDegrees = rot;
        if (rot == 0)
            SetupInput(new Vector2(-Map.tilesize, -Map.tilesize*1.5f), new Vector2I(-1, -1));
        else if (rot == 90)
        {
            Position = Position + Vector2.One * Map.tilesize/2;
            SetupInput(new Vector2(-Map.tilesize, -Map.tilesize*1.5f), new Vector2I(1, -1));
        } else if (rot == 180)
        {
            SetupInput(new Vector2(-Map.tilesize, -Map.tilesize*1.5f), new Vector2I(1, 1));
        } else {
            Position = Position + Vector2.One * Map.tilesize/2;
            SetupInput(new Vector2(-Map.tilesize, -Map.tilesize*1.5f), new Vector2I(1, 1));
        }
    }

    public void AreaEntered(Area2D other)
	{
		if (other.Owner is Item item)
		{
            if (item.type == type)
            {
			    timer.Stop();
                timer.Start();
            }
            item.QueueFree();
		}
	}

    public void Timeout()
    {
        GD.Print("you lost");
        // GameLost();
    }

    public override void Pause(bool isPaused)
    {
        timer.Paused = isPaused;
    }

    public delegate void GameLostEventHandler();
    public GameLostEventHandler GameLost;
}