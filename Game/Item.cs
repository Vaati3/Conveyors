using Godot;
using System;

public enum ItemType {
    Circle = 1,
    Triangle = 3,
    Square,
    Pentagon,
    Hexagon
}

public partial class Item : Node2D
{
    public ItemType type {get; private set;}
	protected Sprite2D sprite;

	public Item(ItemType type)
	{
        this.type = type;

        GD.Print((int)type);
	}
}
