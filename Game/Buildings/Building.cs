using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Building : Node2D
{
	public Vector2I pos {private set; get;}
	public Vector2I input{protected set; get;}
	protected List<Belt> output {private set; get;}
	public Vector2I size {protected set; get;} = Vector2I.One;

	protected Sprite2D sprite;
	protected Area2D area;
	protected bool isPaused;

	public PlaceMode mode = PlaceMode.Remove;

	public Building(Vector2I pos, string textureName, OutputCreatedEventHandler outputCreated)
	{
		this.pos = pos;
		Position = pos * Map.tilesize;
		OutputCreated = outputCreated;

		sprite = new Sprite2D(); 
        sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/" + textureName + ".png");
        AddChild(sprite);

		area = new Area2D();
		sprite.AddChild(area);
		area.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = sprite.Texture.GetSize() - Vector2.One * 64f,
			}
		});

		output = new List<Belt>();
	}

	protected void AddOutput(Vector2I outputPos)
	{
		Belt belt = new Belt(pos + outputPos, null, null);//missing synchro
		belt.building = this;
		output.Add(belt);
		OutputCreated(belt);
	}

	protected void AddInput(Vector2 position, Vector2I inputPos)
	{
		Area2D inputArea = new Area2D();
		inputArea.Position = position;
		AddChild(inputArea);
		inputArea.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = new Vector2(5, 5)
			}
		});
		inputArea.AreaEntered += InputAreaBeltDettect;
		inputArea.AreaExited += InputAreaBeltExit;
		input = inputPos;
	}

	public void InputAreaBeltDettect(Area2D other)
    {
        if (other.Owner is Belt belt)
        {
            belt.Connect(this);
        }
    }

	public void InputAreaBeltExit(Area2D other)
	{
		if (other.Owner is Belt belt)
        {
			belt.OutputLost(true);
		}
	}

	public abstract void Pause(bool isPaused);
	public virtual void Remove(Dictionary<Vector2I, Node2D> nodes)
	{
		foreach(Belt belt in output)
		{
			nodes.Remove(belt.pos);
			belt.Remove();
		}
		QueueFree();
	}

	public delegate void OutputCreatedEventHandler(Belt belt);
	public OutputCreatedEventHandler OutputCreated;
}