using Godot;
using System;
using System.Collections.Generic;

public enum BeltType {
	Dot, Bottom, Top, Right, Left,
	ToBottom, ToTop, ToRight, ToLeft,
    BottomTop, TopToBottom, BottomToTop,
    RightLeft, LeftToRight, RightToLeft,
    BottomLeft, LeftToBottom, BottomToLeft,
    BottomRight, RightToBottom, BottomToRight,
    TopLeft, TopToLeft, LeftToTop,
    TopRight, TopToRight, RightToTop,
	TBottom, TBottomToTop, TBottomToRight, TBottomToLeft,
	TTop, TTopToBottom, TTopToRight, TTopToLeft,
	TRight, TRightToBottom, TRightToTop, TRightToLeft,
	TLeft, TLeftToBottom, TLeftToTop, TLeftToRight,
	Cross, CrossToBottom, CrossToTop, CrossToRight, CrossToLeft,
}
 
public partial class Belt : Node2D
{
    public Vector2I pos {get; private set;}
	public BeltType type {private set; get;}
	public AnimatedSprite2D sprite {get; private set;}
	private Area2D area;
	private int maxItems = 2;

	readonly static BeltType[,] typeMatrix = new BeltType[39, 4]{
		{BeltType.Top, BeltType.Bottom, BeltType.Left, BeltType.Right},
		{BeltType.BottomTop, BeltType.BottomTop, BeltType.BottomLeft, BeltType.BottomRight},
		{BeltType.BottomTop, BeltType.BottomTop, BeltType.TopLeft, BeltType.TopRight},
		{BeltType.TopRight, BeltType.BottomRight, BeltType.RightLeft, BeltType.RightLeft},
		{BeltType.TopLeft, BeltType.BottomLeft, BeltType.RightLeft, BeltType.RightLeft},
		{BeltType.ToTop, BeltType.ToBottom, BeltType.ToLeft, BeltType.ToRight},
		{BeltType.ToTop, BeltType.ToBottom, BeltType.ToLeft, BeltType.ToRight},
		{BeltType.ToTop, BeltType.ToBottom, BeltType.ToLeft, BeltType.ToRight},
		{BeltType.ToTop, BeltType.ToBottom, BeltType.ToLeft, BeltType.ToRight},
		{BeltType.BottomTop, BeltType.BottomTop, BeltType.TLeft, BeltType.TLeft},
		{BeltType.BottomTop, BeltType.BottomTop, BeltType.TLeft, BeltType.TLeft}, // add anim
		{BeltType.BottomTop, BeltType.BottomTop, BeltType.TLeft, BeltType.TLeft}, // add anim
		{BeltType.TBottom, BeltType.TTop, BeltType.RightLeft, BeltType.RightLeft},
		{BeltType.TBottom, BeltType.TTop, BeltType.RightLeft, BeltType.RightLeft}, // add anim
		{BeltType.TBottom, BeltType.TTop, BeltType.RightLeft, BeltType.RightLeft}, // add anim
		{BeltType.TRight, BeltType.BottomLeft, BeltType.BottomLeft, BeltType.TTop},
		{BeltType.TRight, BeltType.BottomLeft, BeltType.BottomLeft, BeltType.TTop}, // add anim
		{BeltType.TRight, BeltType.BottomLeft, BeltType.BottomLeft, BeltType.TTop}, // add anim
		{BeltType.TLeft, BeltType.BottomRight, BeltType.TTop, BeltType.BottomRight},
		{BeltType.TLeft, BeltType.BottomRight, BeltType.TTop, BeltType.BottomRight}, // add anim
		{BeltType.TLeft, BeltType.BottomRight, BeltType.TTop, BeltType.BottomRight}, // add anim
		{BeltType.TopLeft, BeltType.TRight, BeltType.TopLeft, BeltType.TBottom},
		{BeltType.TopLeft, BeltType.TRight, BeltType.TopLeft, BeltType.TBottom}, // add anim
		{BeltType.TopLeft, BeltType.TRight, BeltType.TopLeft, BeltType.TBottom}, // add anim
		{BeltType.TopRight, BeltType.TLeft, BeltType.TBottom, BeltType.TopRight},
		{BeltType.TopRight, BeltType.TLeft, BeltType.TBottom, BeltType.TopRight}, // add anim
		{BeltType.TopRight, BeltType.TLeft, BeltType.TBottom, BeltType.TopRight}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross},
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross},
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross},
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}, // add anim
		{BeltType.Cross, BeltType.Cross, BeltType.Cross, BeltType.Cross}  // add anim
	};

	public Belt(Vector2I pos, Belt synchro, Belt previousBelt)
	{
        this.pos = pos;
		Position = pos * Map.tilesize;
        SetBeltType(previousBelt);

        sprite = new AnimatedSprite2D(){
            SpriteFrames = GD.Load<SpriteFrames>("res://Game/Belts/BeltAnim.tres")
        };
        AddChild(sprite);
        sprite.Play(type.ToString());
        sprite.Animation = type.ToString();
        if (synchro != null)//remove when syncho become a sprite
            sprite.SetFrameAndProgress(synchro.sprite.Frame, synchro.sprite.FrameProgress);

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

	public void Pause(bool isPaused)
	{
		if (isPaused)
		{
			sprite.Pause();
		} else {
			sprite.Play();
		}
	}

	private Vector2 GetItemDirection()
    {
        switch(type)
		{
			case BeltType.LeftToBottom: case BeltType.RightToBottom: case BeltType.TopToBottom:
				return Vector2.Down;
			case BeltType.BottomToLeft: case BeltType.RightToLeft: case BeltType.TopToLeft:
				return Vector2.Left;
			case BeltType.BottomToRight: case BeltType.LeftToRight: case BeltType.TopToRight:
				return Vector2.Right;
			case BeltType.BottomToTop: case BeltType.LeftToTop: case BeltType.RightToTop:
				return Vector2.Up;
		}
		return Vector2.Zero;
    }

    public void ChangeType(BeltType type)
    {
        this.type = type;
        int frame = sprite.Frame;
        float frameProgress = sprite.FrameProgress;
        sprite.Animation = type.ToString();
        sprite.SetFrameAndProgress(frame, frameProgress);
    }

	private void Update(BeltType other)
	{
		//GD.Print("prev " + type + " " + (int)type + " other " + other + " " + (int)(other-1));
		ChangeType(typeMatrix[(int)type, (int)other-1]);
	}

	private void Update(Vector2I other)
	{
		if (other.X != pos.X)
		{
			if (other.X > pos.X)
				ChangeType(typeMatrix[(int)type, 3]);
			else 
				ChangeType(typeMatrix[(int)type, 2]);
		}
		else if (other.Y > pos.Y)
			ChangeType(typeMatrix[(int)type, 1]);
		else 
			ChangeType(typeMatrix[(int)type, 0]);
	}

    public void SetBeltType(Belt previousBelt)
	{
		if (previousBelt == null)
		{
			type = BeltType.Dot;
			return;
		}

		if (previousBelt.pos.X != pos.X)
		{
			if (previousBelt.pos.X > pos.X)
				type = BeltType.Right;
			else 
				type = BeltType.Left;
		}
		else if (previousBelt.pos.Y > pos.Y)
			type = BeltType.Bottom;
		else 
			type = type = BeltType.Top;
		
		previousBelt.Update(type);
		// previousBelt.Update(pos);
	}
}
