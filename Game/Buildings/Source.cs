using System.Collections.Generic;
using Godot;

public partial class Source : Building
{
    ItemType type;
    Timer timer;
    static float itemTime = 2.5f;

    public Source(Vector2I pos, ItemType type, ItemCreatedEventHandler itemCreated) : base(pos, type.ToString() + "Source", false)
    {
        this.type = type;
        ItemCreated += itemCreated;
        size = new Vector2I(3, 2);

        timer = new Timer(){
            Autostart = true,
            OneShot = false,
            WaitTime = itemTime
        };
        timer.Timeout += CreateItem;
        AddChild(timer);
    }

    bool justOne = false;//debug
    private void CreateItem()
    {   
        if (!(GetNodeAt(pos+output) is Belt))//check if belt is full
            return;
        Item item = new Item(type);
        item.Position = (pos + output) * Map.tilesize;
        item.GetNodeAt = GetNodeAt;
        ItemCreated(item);
        
        if (justOne)
            timer.Stop();
    }

    public override void Pause(bool isPaused)
    {
        timer.Paused = isPaused;
    }

    public delegate void ItemCreatedEventHandler(Item item);
    ItemCreatedEventHandler ItemCreated;
}