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
    public Vector2 direction;
	private Sprite2D sprite;

	public Item(ItemType type)
	{
        this.type = type;
        direction = Vector2.Right;

        sprite = new Sprite2D(){
            Scale = new Vector2(0.2f, 0.2f)
        };
        
        sprite.Texture = GD.Load<Texture2D>("res://Game/Items/" + type.ToString() + ".png");
        AddChild(sprite);
	}
}
