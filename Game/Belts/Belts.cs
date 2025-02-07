using Godot;
using System;
using System.Collections.Generic;

public enum BeltInput{
	None = -1, Bottom, Top, Right, Left 
}
 
public partial class Belt : Node2D
{
    public Vector2I pos {get; private set;}
	public AnimatedSprite2D sprite {get; private set;}
	private AnimatedSprite2D synchro;
	private Area2D area;
	private int maxItems = 2;
	List<Belt> previousBelts;

	bool[] inputs;
	BeltInput output = BeltInput.None;

	public Belt(Vector2I pos, AnimatedSprite2D synchro, Belt previousBelt)
	{
        this.pos = pos;
		Position = pos * Map.tilesize;
		previousBelts = new List<Belt>();
		previousBelts.Add(previousBelt);

		this.synchro = synchro;
        sprite = new AnimatedSprite2D(){
            SpriteFrames = GD.Load<SpriteFrames>("res://Game/Belts/BeltAnim.tres")
        };
        AddChild(sprite);
		inputs = new bool[4]{false, false, false, false};
        SetBeltType(previousBelt);
		sprite.Play();

		area = new Area2D();
		AddChild(area);
		area.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = new Vector2(1, 1)
			}
		});
		area.Owner = this;
		area.AreaEntered += AreaEntered;
	}

	private void SetBeltType(Belt previousBelt)
	{
		if (previousBelt == null)
			return;

		Update(previousBelt.pos);
		previousBelt.Update(pos);
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
        // switch(type)
		// {
		// 	case BeltType.LeftToBottom: case BeltType.RightToBottom: case BeltType.TopToBottom:
		// 		return Vector2.Down;
		// 	case BeltType.BottomToLeft: case BeltType.RightToLeft: case BeltType.TopToLeft:
		// 		return Vector2.Left;
		// 	case BeltType.BottomToRight: case BeltType.LeftToRight: case BeltType.TopToRight:
		// 		return Vector2.Right;
		// 	case BeltType.BottomToTop: case BeltType.LeftToTop: case BeltType.RightToTop:
		// 		return Vector2.Up;
		// }
		return Vector2.Zero;
    }

	private void UpdateAnimation()
	{
		if (!inputs[0] && !inputs[1] && !inputs[2] && !inputs[3] && output == BeltInput.None)
		{
			sprite.Animation = "Dot";
			return;
		}
		
		string anim = "";

		if (inputs[(int)BeltInput.Bottom])
			anim += "Bottom";
		if (inputs[(int)BeltInput.Top])
			anim += "Top";
		if (inputs[(int)BeltInput.Right])
			anim += "Right";
		if (inputs[(int)BeltInput.Left])
			anim += "Left";

		if (output != BeltInput.None)
			anim += "To" + output.ToString();

		sprite.Animation = anim;
		sprite.SetFrameAndProgress(synchro.Frame, synchro.FrameProgress);
	}

    // public void ChangeType()
    // {
    //     // this.type = type;
    //     // int frame = sprite.Frame;
    //     // float frameProgress = sprite.FrameProgress;
        
    //     // sprite.SetFrameAndProgress(frame, frameProgress);
    // }


	private void Update(Vector2I other)
	{
		if (other.X != pos.X)
		{
			if (other.X > pos.X)
				inputs[(int)BeltInput.Right] = true;
			else 
				inputs[(int)BeltInput.Left] = true;
		}
		else if (other.Y > pos.Y)
			inputs[(int)BeltInput.Bottom] = true;
		else 
			inputs[(int)BeltInput.Top] = true;

		UpdateAnimation();
	}
	
	private void UpdateLine(Belt nextBelt)
	{
		if (nextBelt.pos.X != pos.X)
		{
			if (nextBelt.pos.X > pos.X)
				output = BeltInput.Right;
			else
				output = BeltInput.Left;
		} 
		else if (nextBelt.pos.Y > pos.Y) 
			output = BeltInput.Bottom;
		else
			output = BeltInput.Top;
		inputs[(int)output] = false;
		UpdateAnimation();

		for(int i = 0; i < previousBelts.Count; i++)
		{
			previousBelts[i]?.UpdateLine(this);
		}
	}

	public void Connect(Building building)
	{
		Vector2I inputPos = building.pos+building.input;

		if (inputPos.X != pos.X)
		{
			if (inputPos.X > pos.X)
				output = BeltInput.Right;
			else
				output = BeltInput.Left;
		} 
		else if (inputPos.Y > pos.Y) 
			output = BeltInput.Bottom;
		else
			output = BeltInput.Top;
		inputs[(int)output] = false;
		
		UpdateAnimation();
		for(int i = 0; i < previousBelts.Count; i++)
		{
			previousBelts[i].UpdateLine(this);
		}
	}
}
