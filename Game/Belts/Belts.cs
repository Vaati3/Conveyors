using Godot;
using System;
using System.Collections.Generic;

public enum Direction {
    BottomTop, TopBottom,
    RightLeft, LeftRight,
    BottomLeft, LeftBottom,
    BottomRight, RightBottom,
    TopLeft, LeftTop,
    TopRight, RightTop
}

public partial class Belt : Node2D
{
	public Direction direction {private set; get;}
	protected AnimatedSprite2D sprite;
    protected Queue<Item> items;

	public Belt(Direction direction)
	{
        this.direction = direction;
        sprite = new AnimatedSprite2D(){
            SpriteFrames = GD.Load<SpriteFrames>("res://Game/Belts/BeltAnim.tres"),
            Scale = new Vector2(0.2f, 0.2f)
        };
        AddChild(sprite);
        sprite.Play(direction.ToString());
	}

    public override void _Process(double delta)
    {
        // MoveItems(); 
    }

    // protected abstract void MoveItems();

    // public abstract void GiveItem(Item item);
}
