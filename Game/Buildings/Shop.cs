using System.Collections.Generic;
using Godot;

public partial class Shop : Building
{
    Timer timer;
    ItemType type;

    int itemNeeded = 0;
    int itemLimit = 8;
    Label itemNeededLabel;
    
    public Shop(Vector2I pos, InternalBeltCreatedEventHandler outputCreated, ItemType type, int rot) : base(pos, type.ToString() + "Shop", outputCreated)
    {
        this.type = type;
        timer = new Timer(){
            Autostart = true,
            WaitTime = Source.itemTime + 1,
            OneShot = false,
        };
        timer.Timeout += Timeout;
        AddChild(timer);

        area.AreaEntered += AreaEntered;
        isRemovable = false;

        itemNeededLabel = new Label();
        AddChild(itemNeededLabel);
        // itemNeededLabel.HorizontalAlignment = HorizontalAlignment.Center;
        // itemNeededLabel.VerticalAlignment = VerticalAlignment.Center;
        itemNeededLabel.RotationDegrees = -rot;
        itemNeededLabel.Set("theme_override_fonts/font", GD.Load<Font>("res://Menus/Themes/Audiowide-Regular.ttf"));
        itemNeededLabel.Set("theme_override_font_sizes/font_size", 100);
        itemNeededLabel.Set("theme_override_constants/line_spacing", -50);
        // itemNeededLabel.Position = new Vector2I(10, 30);

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

        UpdateLabel();
    }

    public void AreaEntered(Area2D other)
	{
		if (other.Owner is Item item)
		{
            if (item.type == type)
            {
			    itemNeeded -= itemNeeded - 1 < 0 ? 0 : 1;
                ScoreUpdated(1);
            } else {
                itemNeeded++;
                ScoreUpdated(-1);
                if (itemNeeded > itemLimit)
                    GameLost();
            }
            UpdateLabel();
            item.QueueFree();
		}
	}

    public void Timeout()
    {
        itemNeeded++;
        UpdateLabel();
        if (itemNeeded > itemLimit)
            GameLost();
    }

    public void Upgrade()
    {
        timer.WaitTime /= 2;
        itemLimit = (int)(itemLimit * 1.5);
        UpdateLabel();
    }

    public override void Pause(bool isPaused)
    {
        timer.Paused = isPaused;
        this.isPaused = isPaused;
    }

    private void UpdateLabel()
    {
        if (RotationDegrees == 90 || RotationDegrees == 270)
            itemNeededLabel.Text = itemNeeded + "\n—\n" + itemLimit;
        else
            itemNeededLabel.Text = itemNeeded + "/" + itemLimit; 
    }

    public delegate void ScoreUpdatedEventHandler(int value);
    public ScoreUpdatedEventHandler ScoreUpdated;

    public override void Rotate(){}

    public delegate void GameLostEventHandler();
    public GameLostEventHandler GameLost;
}