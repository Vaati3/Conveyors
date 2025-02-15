using System.Collections.Generic;
using Godot;

public partial class Source : Building
{
    ItemType type;
    Timer timer;
    public static float itemTime = 5f;

    Node itemLayer;

    public Source(Vector2I pos, InternalBeltCreatedEventHandler outputCreated, ItemType type, Node itemLayer) : base(pos, type.ToString() + "Source", outputCreated)
    {
        this.type = type;
        this.itemLayer = itemLayer;
        isRemovable = false;

        timer = new Timer(){
            Autostart = true,
            OneShot = false,
            WaitTime = itemTime
        };
        timer.Timeout += CreateItem;
        AddChild(timer);

        AddOutput(Vector2I.Zero);
    }

    private void CreateItem()
    {   
        if (output[0].output == BeltInput.None)
            return;
        Item item = new Item(type);
        item.belt = output[0];
        itemLayer.AddChild(item);
        item.Position = pos * Map.tilesize;
    }

    public override void Pause(bool isPaused)
    {
        timer.Paused = isPaused;
        this.isPaused = isPaused;
    }
}