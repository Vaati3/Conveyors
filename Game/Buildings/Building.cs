using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Building : Node2D
{
	public Vector2I pos {private set; get;}
	protected bool hasInput {private set; get;} = false;
	protected Area2D inputArea {private set; get;} = null;
	public Vector2I input{protected set; get;}
	protected bool hasOutput {private set; get;}
	protected Vector2I output {private set; get;} = Vector2I.Zero;

	public Vector2I size {protected set; get;} = Vector2I.One;

	protected Sprite2D sprite;
	protected Area2D area;

	public Building(Vector2I pos, string textureName, bool hasOutput = true)
	{
		this.pos = pos;
		Position = pos * Map.tilesize;

		this.hasInput = hasInput;
		this.hasOutput = hasOutput;

		sprite = new Sprite2D(); 
        sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/" + textureName + ".png");
        AddChild(sprite);

		if (hasOutput)
			output = Vector2I.Down;

		area = new Area2D();
		sprite.AddChild(area);
		area.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = sprite.Texture.GetSize() - Vector2.One * 64f,
			}
		});
	}

	protected void SetupInput(Vector2 position, Vector2I inputPos)
	{
		hasInput = true;
		inputArea = new Area2D();
		inputArea.Position = position;
		AddChild(inputArea);
		inputArea.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = new Vector2(5, 5)
			}
		});
		inputArea.AreaEntered += InputAreaBeltDettect;
		input = inputPos;
	}

	public void InputAreaBeltDettect(Area2D other)
    {
        if (other.Owner is Belt belt)
        {
            belt.Connect(this);
        }
    }

	public abstract void Pause(bool isPaused);

	public delegate Node2D GetNodeAtEventHandler(Vector2I pos);
	public GetNodeAtEventHandler GetNodeAt;
}