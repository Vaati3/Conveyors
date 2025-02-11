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
	public AnimatedSprite2D synchro;
	private Area2D area;
	private Area2D colisionArea;
	public List<Item> items {get; private set;}
	public Belt[] otherBelts;

	public bool[] inputs {get; private set;}
	public BeltInput output {get; private set;} = BeltInput.None;
	public int speed {get; private set;} = 50;

	public Building building = null;

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
				Size = Vector2.One
			}
		});
		area.Owner = this;
		area.AreaEntered += AreaEntered;
		
		items = new List<Item>();
		colisionArea = new Area2D();
		AddChild(colisionArea);
		colisionArea.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = Vector2.One * Map.tilesize
			}
		});
		colisionArea.AreaEntered += ColisionAreaEntered;
		colisionArea.AreaExited += ColisionAreaExited;
		SetBeltType(previousBelt);
	}

	private void SetBeltType(Belt previousBelt)
	{
		if (previousBelt == null)
		{
			UpdateAnimation();
			return;
		}
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

	private BeltInput GetOutput(Vector2I other)
	{
		if (other.X != pos.X)
		{
			if (other.X > pos.X)
				return BeltInput.Right;
			return BeltInput.Left;
		} 
		if (other.Y > pos.Y) 
			return BeltInput.Bottom;
		return BeltInput.Top;
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
		output = GetOutput(inputPos);
		UpdateAnimation();
		for(int i = 0; i <= (int)BeltInput.Left; i++)
		{
			if (otherBelts[i] == null)
				continue;
			otherBelts[i].UpdateLine(this);
		}
	}

	public void Connect(Belt belt)
	{
		if (output != BeltInput.None && belt.output != BeltInput.None)
			return;

		if (belt.output != BeltInput.None)
		{
			output = GetOutput(belt.pos);
			UpdateAnimation();
			belt.otherBelts[(int)belt.GetOutput(pos)] = this;
			belt.Update(this);

			for(int i = 0; i <= (int)BeltInput.Left; i++)
			{
				if (otherBelts[i] == null)
					continue;
				otherBelts[i].UpdateLine(this);
			}
			return;
		}

		if (output != BeltInput.None)
		{
			belt.Connect(this);
			return;
		}

		otherBelts[(int)GetOutput(belt.pos)] = belt;
		belt.otherBelts[(int)belt.GetOutput(pos)] = this;
		Update(belt);
		belt.Update(this);
	}

	public void Remove()
	{
		for(int i = items.Count-1; i >= 0; i--)
			items[i].QueueFree();
		for(int i = 0; i <= (int)BeltInput.Left; i++)
		{
			if (otherBelts[i] == null)
				continue;
			otherBelts[i].Remove(this);
		}
		QueueFree();
	}

	public void Remove(Belt belt)
	{
		int i = (int)GetOutput(belt.pos);
		inputs[i] = false;
		otherBelts[i] = null;
		if (output != BeltInput.None && output == GetOutput(belt.pos)) 
		{
			OutputLost(true);
			return;
		}
		
		UpdateAnimation();
	}

	public void OutputLost(bool first = false)
	{
		for(int i = 0; i <= (int)BeltInput.Left; i++)
		{
			if (otherBelts[i] == null || i == (int)output)
				continue;
			otherBelts[i].OutputLost();
		}
		inputs[(int)output] = !first;
		output = BeltInput.None;
		UpdateAnimation();
	}

	public void Pause(bool isPaused)
	{
		if (isPaused)
		{
			sprite.Pause();
		} else {
			sprite.Play();
		}
		foreach (Item item in items)
        {
            item.isPaused = isPaused;
        }
	}

	public void AreaEntered(Area2D other)
	{
		if (other.Owner is Item item)
		{
			item.belt = this;
		}
	}

	public void ColisionAreaEntered(Area2D other)
	{
		if (other.Owner is Item item)
		{
			items.Add(item);
		}
	}
	public void ColisionAreaExited(Area2D other)
	{
		if (other.Owner is Item item)
		{
			items.Remove(item);
		}
	}
}
