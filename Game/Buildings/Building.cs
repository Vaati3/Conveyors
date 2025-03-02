using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Building : Node2D
{
	public Vector2I pos {private set; get;}
	public List<Belt> input {private set; get;}
	public List<Belt> output {private set; get;}
	public Vector2I size {protected set; get;} = Vector2I.One;

	protected Sprite2D sprite;
	protected Area2D area;
	protected bool isPaused;

	public PlaceMode mode = PlaceMode.Remove;
	public bool isRemovable {protected set; get;}= true;

	bool grow = true;
	float weight;
	float speed = 0.05f;

    public override void _Process(double delta)
    {
		if (grow)
		{
        	weight += speed + (float)delta;
			float value = Mathf.Lerp(1.1f, 1.2f, weight);
			sprite.Scale = new Vector2(value, value);
			if (weight >= 1)
			{
				weight = 0;
				grow = false;
			}

		} else if ( weight < 1)
		{
			weight += speed + (float)delta;
			float value = Mathf.Lerp(1.2f, 1, weight);
			sprite.Scale = new Vector2(value, value);
		}
    }

    public Building(Vector2I pos, string textureName, InternalBeltCreatedEventHandler outputCreated)
	{
		this.pos = pos;
		Position = pos * Map.tilesize;
		InternalBeltCreated = outputCreated;

		sprite = new Sprite2D(); 
        sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/" + textureName + ".png");
        AddChild(sprite);

		area = new Area2D();
		sprite.AddChild(area);
		area.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = sprite.Texture.GetSize() - Vector2.One * 86f,
			}
		});

		output = new List<Belt>();
		input = new List<Belt>();
	}

	protected void AddOutput(Vector2I outputPos)
	{
		Belt belt = new Belt(pos + outputPos, null, null, null);
		belt.building = this;
		output.Add(belt);
		InternalBeltCreated(belt);
	}

	protected void AddInput(Vector2I inputPos, BeltInput beltType)
	{
		Belt belt = new Belt(pos + inputPos, null, null, null);
		belt.building = this;
		belt.isBuildingInput = true;
		input.Add(belt);
		InternalBeltCreated(belt);
		belt.Connect(beltType);
	}

	public void StopAnim()
	{
		grow = false;
		weight = 2;
		sprite.Scale = Vector2.One;
	}

	public abstract void Pause(bool isPaused);

	public virtual void Remove(){}

	public virtual void RotateBuilding(float angle)
	{
		sprite.Rotation += angle;
	}

	public delegate void InternalBeltCreatedEventHandler(Belt belt);
	public InternalBeltCreatedEventHandler InternalBeltCreated;
}