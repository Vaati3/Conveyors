using System.Collections.Generic;
using Godot;

public partial class Source : Building
{
    ItemType type;
    Timer timer;
    static float itemTime = 2.5f;

    Node itemLayer;

    public Source(Vector2I pos, OutputCreatedEventHandler outputCreated, ItemType type, Node itemLayer) : base(pos, type.ToString() + "Source", outputCreated)
    {
        this.type = type;
        this.itemLayer = itemLayer;

        timer = new Timer(){
            Autostart = true,
            OneShot = false,
            WaitTime = itemTime
        };
        timer.Timeout += CreateItem;
        AddChild(timer);
        // sprite.Visible =false;

        AddOutput(Vector2I.Zero);
    }

    private void CreateItem()
    {   
        if (output[0].output == BeltInput.None)//check if belt is full
            return;
        Item item = new Item(type);
        item.belt = output[0];
        itemLayer.AddChild(item);
        item.Position = pos * Map.tilesize;
    }

    public override void Pause(bool isPaused)
    {
        timer.Paused = isPaused;
    }
}