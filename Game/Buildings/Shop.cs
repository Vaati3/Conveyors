using Godot;

public partial class Shop : Building
{
    Timer timer;
    ItemType type;
    
    public Shop(Vector2I pos, ItemType type, int rot) : base(pos, type.ToString() + "Shop", false)
    {
		// Position = Position - Vector2.Down * Map.tilesize/2;
        this.type = type;
        timer = new Timer(){
            Autostart = true,
            WaitTime = 20,
            OneShot = true,
        };
        timer.Timeout += Timeout;
        AddChild(timer);

        area.AreaEntered += AreaEntered;

        rot = 270;
        RotationDegrees = rot;
        if (rot == 0){
            size = new Vector2I(3, 2);
            sprite.Position = new Vector2(Map.tilesize, Map.tilesize * 0.5f);
            SetupInput(new Vector2(0, -Map.tilesize), new Vector2I(0, 1));
        } else if (rot == 90)
        {
            size = new Vector2I(2, 3);
            sprite.Position = new Vector2(Map.tilesize, Map.tilesize * -0.5f);
            SetupInput(new Vector2(0, -Map.tilesize * 2), new Vector2I(1, -1));
        } else if (rot == 180)
        {
            size = new Vector2I(3, 2);
            sprite.Position = new Vector2(-Map.tilesize, Map.tilesize * -0.5f);
            SetupInput(new Vector2(Map.tilesize*-2, Map.tilesize*-2), new Vector2I(2, 1));
        } else if (rot == 270){
            size = new Vector2I(2, 3);
            sprite.Position = new Vector2(-Map.tilesize, Map.tilesize *0.5f);
            SetupInput(new Vector2(Map.tilesize*-2, -Map.tilesize), new Vector2I(1, 1));
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