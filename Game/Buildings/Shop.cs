using System;
using System.Collections.Generic;
using Godot;

public partial class Shop : Building
{
    Timer timer;
    ItemType type;

    int itemNeeded = 8;
    int maxItem = 8;
    Label itemNeededLabel;
    int level = 1;

    bool isDemo;
    
    public Shop(Vector2I pos, InternalBeltCreatedEventHandler outputCreated, ItemType type, bool isDemo = false) : base(pos, type.ToString() + "Shop", outputCreated)
    {
        this.isDemo = isDemo;
        sprite.Position = new Vector2(Map.tilesize * 0.5f, Map.tilesize * 0.5f);
        this.type = type;
        timer = new Timer(){
            Autostart = true,
            WaitTime = Source.itemTime + 2,
            OneShot = false,
        };
        timer.Timeout += Timeout;
        AddChild(timer);

        area.AreaEntered += AreaEntered;
        isRemovable = false;

        itemNeededLabel = new Label();
        itemNeededLabel.Position = new Vector2(30, -30);
        AddChild(itemNeededLabel);
        itemNeededLabel.Set("theme_override_fonts/font", GD.Load<Font>("res://Menus/Themes/Audiowide-Regular.ttf"));
        itemNeededLabel.Set("theme_override_font_sizes/font_size", 75);
        itemNeededLabel.Set("theme_override_constants/line_spacing", -50);

        size = new Vector2I(2, 2);
        AddInput(new Vector2I(0, 0), BeltInput.Right);
        UpdateLabel();
    }

    public void AreaEntered(Area2D other)
	{
		if (other.Owner is Item item)
		{
            if (item.type == type)
            {
			    itemNeeded += itemNeeded + 1 > maxItem ? 0 : 1;
                if (!isDemo)
                    ScoreUpdated(1);
            } else {
                itemNeeded--;
                if (!isDemo)
                {
                    ScoreUpdated(-1);
                    if (itemNeeded > maxItem)
                        GameLost();
                }
            }
            UpdateLabel();
            item.QueueFree();
		}
	}

    public void Timeout()
    {
        itemNeeded--;
        UpdateLabel();
        if (itemNeeded <= 0)
            GameLost();
    }

    public void Upgrade()
    {
        level++;
        timer.WaitTime = (Source.itemTime + 1) / level;
        maxItem += 2;
        itemNeeded += 2;
        UpdateLabel();
    }

    public override void Pause(bool isPaused)
    {
        timer.Paused = isPaused;
        this.isPaused = isPaused;
    }

    private void UpdateLabel()
    {
        itemNeededLabel.Text = itemNeeded + "\n—\n" + maxItem;
    }

    public delegate void ScoreUpdatedEventHandler(int value);
    public ScoreUpdatedEventHandler ScoreUpdated;

    public delegate void GameLostEventHandler();
    public GameLostEventHandler GameLost;
}