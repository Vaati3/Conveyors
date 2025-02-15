using System;
using System.Collections.Generic;
using Godot;

public partial class Shop : Building
{
    Timer timer;
    ItemType type;

    int itemNeeded = 0;
    int itemLimit = 8;
    Label itemNeededLabel;
    
    public Shop(Vector2I pos, InternalBeltCreatedEventHandler outputCreated, ItemType type, float angle) : base(pos, type.ToString() + "Shop", outputCreated)
    {
        sprite.Position = new Vector2(Map.tilesize * 0.5f, Map.tilesize * 0.5f);
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
        itemNeededLabel.Position = new Vector2(75, -25);
        AddChild(itemNeededLabel);
        itemNeededLabel.Set("theme_override_fonts/font", GD.Load<Font>("res://Menus/Themes/Audiowide-Regular.ttf"));
        itemNeededLabel.Set("theme_override_font_sizes/font_size", 75);
        itemNeededLabel.Set("theme_override_constants/line_spacing", -50);

        size = new Vector2I(2, 2);
        if (angle == MathF.PI || angle == 3 * (MathF.PI/2))
            AddInput(new Vector2I(0, 0), BeltInput.Top);
        else
            AddInput(new Vector2I(0, 0), BeltInput.Bottom);
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
        itemNeededLabel.Text = itemNeeded + "\n—\n" + itemLimit;
        // itemNeededLabel.Text = itemNeeded + "/" + itemLimit; 
    }

    public delegate void ScoreUpdatedEventHandler(int value);
    public ScoreUpdatedEventHandler ScoreUpdated;

    public override void RotateBuilding(float angle)
    {
        base.RotateBuilding(angle);
        
        if (angle == MathF.PI/2 || angle == MathF.PI)
            itemNeededLabel.Position = new Vector2(-25, -25);
    }

    public delegate void GameLostEventHandler();
    public GameLostEventHandler GameLost;
}