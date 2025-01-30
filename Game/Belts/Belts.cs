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
    public Vector2I pos {get; private set;}
	public Direction direction {private set; get;}
	public AnimatedSprite2D sprite {get; private set;}
    private List<Item> items;
	private int maxItems = 4;
	private float speed = 50;

	public Belt(Vector2I pos, Direction direction, Belt previousBelt)
	{
		items = new List<Item>();
        this.pos = pos;
		Position = pos * Map.tilesize;
        this.direction = direction;
        sprite = new AnimatedSprite2D(){
            SpriteFrames = GD.Load<SpriteFrames>("res://Game/Belts/BeltAnim.tres"),
            Scale = new Vector2(0.2f, 0.2f)
        };
        AddChild(sprite);
        sprite.Play(direction.ToString());
        sprite.Animation = direction.ToString();
        if (previousBelt != null)
            sprite.SetFrameAndProgress(previousBelt.sprite.Frame, previousBelt.sprite.FrameProgress);
	}

    public override void _Process(double delta)
    {
		foreach(Item item in items)
		{
			item.Position += item.direction * speed * (float)delta;
		}
    }

    public bool ReceiveItem(Item item)
	{
		if (items.Count >= maxItems)
			return false;
		items.Add(item);
		return true;
	}

    public void ChangeDirection(Direction direction)
    {
        this.direction = direction;
        int frame = sprite.Frame;
        float frameProgress = sprite.FrameProgress;
        sprite.Animation = direction.ToString();
        sprite.SetFrameAndProgress(frame, frameProgress);
    }

    public static Direction GetBeltDirection(Vector2I pos, Vector2I dir, Belt previousBelt)
	{
		if (previousBelt == null)
		{
			if (dir.X != 0)
        	{
            	if (dir.X > 0)
                	return Direction.LeftRight;
                return Direction.RightLeft;
        	}
            if (dir.Y > 0)
                return Direction.BottomTop; 
            return Direction.TopBottom;
		}
		
		switch (previousBelt.direction)
		{
			case Direction.TopBottom:
				if (previousBelt.pos.X != pos.X)
				{
					if (previousBelt.pos.X > pos.X)
					{
						previousBelt.ChangeDirection(Direction.TopLeft);
						return Direction.RightLeft;
					}
					previousBelt.ChangeDirection(Direction.TopRight);
					return Direction.LeftRight;
				}
				if (previousBelt.pos.Y > pos.Y)
				{
					previousBelt.ChangeDirection(Direction.BottomTop);
					return Direction.BottomTop;
				}
				return Direction.TopBottom;
			case Direction.BottomTop:
				if (previousBelt.pos.X != pos.X)
				{
					if (previousBelt.pos.X > pos.X)
					{
						previousBelt.ChangeDirection(Direction.BottomLeft);
						return Direction.RightLeft;
					}
					previousBelt.ChangeDirection(Direction.BottomRight);
					return Direction.LeftRight;
				}
				if (previousBelt.pos.Y > pos.Y)
					return Direction.BottomTop;
				previousBelt.ChangeDirection(Direction.TopBottom);
				return Direction.TopBottom;
			case Direction.LeftRight:
				if (previousBelt.pos.Y != pos.Y)
				{
					if (previousBelt.pos.Y > pos.Y)
					{
						previousBelt.ChangeDirection(Direction.LeftTop);
						return Direction.BottomTop;
					}
					previousBelt.ChangeDirection(Direction.LeftBottom);
					return Direction.TopBottom;
				}
				if (previousBelt.pos.X > pos.X)
				{
					previousBelt.ChangeDirection(Direction.RightLeft);
					return Direction.RightLeft;
				}
				return Direction.LeftRight;
			case Direction.RightLeft:
				if (previousBelt.pos.Y != pos.Y)
				{
					if (previousBelt.pos.Y > pos.Y)
					{
						previousBelt.ChangeDirection(Direction.RightTop);
						return Direction.BottomTop;
					}
					previousBelt.ChangeDirection(Direction.RightBottom);
					return Direction.TopBottom;
				}
				if (previousBelt.pos.X > pos.X)
					return Direction.RightLeft;
				previousBelt.ChangeDirection(Direction.LeftRight);
				return Direction.LeftRight;
		}
		return 0;
	}
}
