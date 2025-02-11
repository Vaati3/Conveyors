using System.Collections.Generic;
using Godot;

public partial class Shop : Building
{
    Timer timer;
    ItemType type;
    
    public Shop(Vector2I pos, InternalBeltCreatedEventHandler outputCreated, ItemType type, int rot) : base(pos, type.ToString() + "Shop", outputCreated)
    {
        this.type = type;
        timer = new Timer(){
            Autostart = true,
            WaitTime = 20,
            OneShot = true,
        };
        timer.Timeout += Timeout;
        AddChild(timer);
        
        isRemovable = false;

        area.AreaEntered += AreaEntered;

        RotationDegrees = rot;
        if (rot == 0){
            size = new Vector2I(3, 2);
            sprite.Position = new Vector2(Map.tilesize, Map.tilesize * 0.5f);
            AddInput(new Vector2I(0, 0), BeltInput.Bottom);
        } else if (rot == 90)
        {
            size = new Vector2I(2, 3);
            sprite.Position = new Vector2(Map.tilesize, Map.tilesize * -0.5f);
            AddInput(new Vector2I(1, 0), BeltInput.Right);
        } else if (rot == 180)
        {
            size = new Vector2I(3, 2);
            sprite.Position = new Vector2(-Map.tilesize, Map.tilesize * -0.5f);
            AddInput(new Vector2I(2, 1), BeltInput.Left);
        } else if (rot == 270){
            size = new Vector2I(2, 3);
            sprite.Position = new Vector2(-Map.tilesize, Map.tilesize *0.5f);
            AddInput(new Vector2I(0, 2), BeltInput.Top);
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
        this.isPaused = isPaused;
    }

    public override void Rotate(){}

    public delegate void GameLostEventHandler();
    public GameLostEventHandler GameLost;
}