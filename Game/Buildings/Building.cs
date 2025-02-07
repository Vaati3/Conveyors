using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Building : Node2D
{
	public Vector2I pos {private set; get;}
	public Vector2I size {protected set; get;} = Vector2I.One;
	protected bool hasInput {private set; get;}
	protected Area2D inputArea {private set; get;} = null;
	public Vector2I input{protected set; get;}
	protected bool hasOutput {private set; get;}
	protected Vector2I output {private set; get;} = Vector2I.Zero;

	protected Sprite2D sprite;
	protected Area2D area;

	public Building(Vector2I pos, string textureName, bool hasInput = true, bool hasOutput = true)
	{
		this.pos = pos;
		Vector2 newPosition = pos * Map.tilesize;
		newPosition.Y -= Map.tilesize/2;
		Position = newPosition;

		this.hasInput = hasInput;
		this.hasOutput = hasOutput;

		sprite = new Sprite2D(); 
        sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/" + textureName + ".png");
        AddChild(sprite);

		if (hasInput)
		{
			inputArea = new Area2D();
			inputArea.Position = new Vector2(-Map.tilesize, -Map.tilesize - 64);
			AddChild(inputArea);
			inputArea.AddChild(new CollisionShape2D(){
				Shape = new RectangleShape2D() {
					Size = new Vector2(10, 10)
				}
			});
			inputArea.AreaEntered += InputAreaBeltDettect;
			input = new Vector2I(-1, -1);
		}
		if (hasOutput)
			output = Vector2I.Down;

		area = new Area2D();
		AddChild(area);
		area.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = new Vector2(1, 1)
			}
		});
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
