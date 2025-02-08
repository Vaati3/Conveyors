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
	public Belt[] otherBelts;

	public bool[] inputs {get; private set;}
	public BeltInput output {get; private set;} = BeltInput.None;

	public Belt(Vector2I pos, AnimatedSprite2D synchro, Belt previousBelt)
	{
        this.pos = pos;
		Position = pos * Map.tilesize;
		otherBelts = new Belt[4];
		inputs = new bool[4]{false, false, false, false};

		this.synchro = synchro;
        sprite = new AnimatedSprite2D(){
            SpriteFrames = GD.Load<SpriteFrames>("res://Game/Belts/BeltAnim.tres")
        };
        AddChild(sprite);
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
		
		SetBeltType(previousBelt);
	}

	private void SetBeltType(Belt previousBelt)
	{
		if (previousBelt == null)
			return;

		if (previousBelt != null && previousBelt.output != BeltInput.None)
		{
			output = GetOutput(previousBelt.pos);
		} 
		otherBelts[(int)GetOutput(previousBelt.pos)] = previousBelt;
		previousBelt.otherBelts[(int)previousBelt.GetOutput(pos)] = this;
		Update(previousBelt);
		previousBelt.Update(this);
	}

	private void UpdateAnimation()
	{
		if (!inputs[0] && !inputs[1] && !inputs[2] && !inputs[3] && output == BeltInput.None)
		{
			sprite.Animation = "Dot";
			return;
		}
		if (output != BeltInput.None)
			inputs[(int)output] = false;

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

	private void Update(Belt other)
	{
		if (other.pos.X != pos.X)
		{
			if (other.pos.X > pos.X)
				inputs[(int)BeltInput.Right] = true;
			else 
				inputs[(int)BeltInput.Left] = true;
		}
		else if (other.pos.Y > pos.Y)
			inputs[(int)BeltInput.Bottom] = true;
		else 
			inputs[(int)BeltInput.Top] = true;

		UpdateAnimation();
	}

	private BeltInput GetOutput(Vector2I other, bool backward = false)
	{
		if (other.X != pos.X)
		{
			if (other.X > pos.X)
				return backward ? BeltInput.Left : BeltInput.Right;
			return backward ? BeltInput.Right : BeltInput.Left;
		} 
		if (other.Y > pos.Y) 
			return backward ? BeltInput.Top : BeltInput.Bottom;
		return backward ? BeltInput.Bottom : BeltInput.Top;
	}
	
	private void UpdateLine(Belt nextBelt)
	{
		output = GetOutput(nextBelt.pos);
		UpdateAnimation();

		for(int i = 0; i <= (int)BeltInput.Left; i++)
		{
			if (otherBelts[i] == null || i == (int)output)
				continue;
			
			otherBelts[i].UpdateLine(this);
		}
	}

	public void Connect(Building building)
	{
		Vector2I inputPos = building.pos+building.input;

		output = GetOutput(pos, true);
		
		UpdateAnimation();
		for(int i = 0; i <= (int)BeltInput.Left; i++)
		{
			if (otherBelts[i] == null)
				continue;
			otherBelts[i].UpdateLine(this);
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

	public void AreaEntered(Area2D other)
	{
		if (other.Owner is Item item)
		{
			item.direction = GetItemDirection();
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
}
