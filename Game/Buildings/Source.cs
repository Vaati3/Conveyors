using Godot;
using System;
using System.Collections.Generic;

public partial class Source : Building
{
    ItemType type;
    Timer timer;
    const float itemTime = 5; 
    public Source(Vector2I pos, ItemType type) : base(pos, "Source" + type.ToString(), false)
    {
        this.type = type;

        timer = new Timer(){
            Autostart = true,
            OneShot = false,
            WaitTime = itemTime
        };
        timer.Timeout += CreateItem;
        AddChild(timer);
    }

    private void CreateItem()
    {
        Item item = new Item(type);
        item.Position = (pos + output) * Map.tilesize;
        if (!SendItem(item, pos + output))
            item.QueueFree();
    }
}