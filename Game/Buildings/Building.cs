using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Building : Node2D
{
	public Vector2I pos {private set; get;}
	protected bool hasInput {private set; get;}
	protected Vector2I input {private set; get;}
	protected bool hasOutput {private set; get;}
	protected Vector2I output {private set; get;}

	protected Sprite2D sprite;
	protected List<Item> items;

	public Building(Vector2I pos, string textureName, bool hasInput = true, bool hasOutput = true)
	{
		this.pos = pos;
		Position = pos * Map.tilesize;
		items = new List<Item>();
		this.hasInput = hasInput;
		this.hasOutput = hasOutput;

		input = Vector2I.Up;
		output = Vector2I.Down;

		sprite = new Sprite2D(){
            Scale = new Vector2(0.2f, 0.2f)
        }; 
        sprite.Texture = GD.Load<Texture2D>("res://Game/Buildings/Textures/" + textureName + ".png");
        AddChild(sprite);
	}

	public bool ReceiveItem(Item item)
	{
		if (!hasInput)
			return false;

		items.Add(item);
		return true;
	}

	public delegate Node2D GetNodeAtEventHandler(Vector2I pos);
	public GetNodeAtEventHandler GetNodeAt;
}
