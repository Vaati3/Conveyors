using Godot;

public partial class Shop : Building
{
    Timer timer;
    ItemType type;
    public Shop(Vector2I pos, ItemType type) : base(pos, type.ToString() + "Shop", false)
    {
        this.type = type;
        timer = new Timer(){
            Autostart = true,
            WaitTime = 20,
            OneShot = true,
        };
        timer.Timeout += Timeout;
        AddChild(timer);

        area.AreaEntered += AreaEntered;
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

    public delegate void GameLostEventHandler();
    public GameLostEventHandler GameLost;
}