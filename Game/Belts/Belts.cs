using Godot;
using System;
using System.Collections.Generic;

public enum BeltType {
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
	public BeltType type {private set; get;}
	public AnimatedSprite2D sprite {get; private set;}
	private Area2D area;
    private List<Item> items;
	private int maxItems = 2;
	private float speed = 50;

	public Belt(Vector2I pos, BeltType type, Belt previousBelt)
	{
		items = new List<Item>();
        this.pos = pos;
		Position = pos * Map.tilesize;
        this.type = type;
        sprite = new AnimatedSprite2D(){
            SpriteFrames = GD.Load<SpriteFrames>("res://Game/Belts/BeltAnim.tres"),
            Scale = new Vector2(0.2f, 0.2f)
        };
        AddChild(sprite);
        sprite.Play(type.ToString());
        sprite.Animation = type.ToString();
        if (previousBelt != null)
            sprite.SetFrameAndProgress(previousBelt.sprite.Frame, previousBelt.sprite.FrameProgress);

		area = new Area2D();
		AddChild(area);
		area.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = new Vector2(1, 1)
			}
		});
		
		area.AreaEntered += AreaEntered;
	}

	public void AreaEntered(Area2D other)
	{
		if (other.Owner is Item item)
		{
			item.direction = GetItemDirection();
		}
	}

	private Vector2 GetItemDirection()
    {
        switch(type)
		{
			case BeltType.LeftBottom: case BeltType.RightBottom: case BeltType.TopBottom:
				return Vector2.Down;
			case BeltType.BottomLeft: case BeltType.RightLeft: case BeltType.TopLeft:
				return Vector2.Left;
			case BeltType.BottomRight: case BeltType.LeftRight: case BeltType.TopRight:
				return Vector2.Right;
			case BeltType.BottomTop: case BeltType.LeftTop: case BeltType.RightTop:
				return Vector2.Up;
		}
		return Vector2.Zero;
    }

    public void ChangeDirection(BeltType direction)
    {
        this.type = direction;
        int frame = sprite.Frame;
        float frameProgress = sprite.FrameProgress;
        sprite.Animation = direction.ToString();
        sprite.SetFrameAndProgress(frame, frameProgress);
    }

    public static BeltType GetBeltDirection(Vector2I pos, Vector2I dir, Belt previousBelt)
	{
		if (previousBelt == null)
		{
			if (dir.X != 0)
        	{
            	if (dir.X > 0)
                	return BeltType.LeftRight;
                return BeltType.RightLeft;
        	}
            if (dir.Y > 0)
                return BeltType.BottomTop; 
            return BeltType.TopBottom;
		}
		
		switch (previousBelt.type)
		{
			case BeltType.TopBottom:
				if (previousBelt.pos.X != pos.X)
				{
					if (previousBelt.pos.X > pos.X)
					{
						previousBelt.ChangeDirection(BeltType.TopLeft);
						return BeltType.RightLeft;
					}
					previousBelt.ChangeDirection(BeltType.TopRight);
					return BeltType.LeftRight;
				}
				if (previousBelt.pos.Y > pos.Y)
				{
					previousBelt.ChangeDirection(BeltType.BottomTop);
					return BeltType.BottomTop;
				}
				return BeltType.TopBottom;
			case BeltType.BottomTop:
				if (previousBelt.pos.X != pos.X)
				{
					if (previousBelt.pos.X > pos.X)
					{
						previousBelt.ChangeDirection(BeltType.BottomLeft);
						return BeltType.RightLeft;
					}
					previousBelt.ChangeDirection(BeltType.BottomRight);
					return BeltType.LeftRight;
				}
				if (previousBelt.pos.Y > pos.Y)
					return BeltType.BottomTop;
				previousBelt.ChangeDirection(BeltType.TopBottom);
				return BeltType.TopBottom;
			case BeltType.LeftRight:
				if (previousBelt.pos.Y != pos.Y)
				{
					if (previousBelt.pos.Y > pos.Y)
					{
						previousBelt.ChangeDirection(BeltType.LeftTop);
						return BeltType.BottomTop;
					}
					previousBelt.ChangeDirection(BeltType.LeftBottom);
					return BeltType.TopBottom;
				}
				if (previousBelt.pos.X > pos.X)
				{
					previousBelt.ChangeDirection(BeltType.RightLeft);
					return BeltType.RightLeft;
				}
				return BeltType.LeftRight;
			case BeltType.RightLeft:
				if (previousBelt.pos.Y != pos.Y)
				{
					if (previousBelt.pos.Y > pos.Y)
					{
						previousBelt.ChangeDirection(BeltType.RightTop);
						return BeltType.BottomTop;
					}
					previousBelt.ChangeDirection(BeltType.RightBottom);
					return BeltType.TopBottom;
				}
				if (previousBelt.pos.X > pos.X)
					return BeltType.RightLeft;
				previousBelt.ChangeDirection(BeltType.LeftRight);
				return BeltType.LeftRight;
		}
		return 0;
	}
}
