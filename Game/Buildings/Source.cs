using Godot;
using System;
using System.Collections.Generic;

public partial class Source : Building
{
    ItemType type;
    Timer timer;
    const float itemTime = 2.5f; 
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

    bool justOne = false;//debug
    private void CreateItem()
    {   
        if (!(GetNodeAt(pos+output) is Belt))//check if belt is full
            return;
        Item item = new Item(type);
        item.Position = (pos + output) * Map.tilesize;
        GetParent().AddChild(item);
        
        if (justOne)
            timer.Stop();
    }
}