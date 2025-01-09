using Godot;
using System;

public abstract partial class Building : Node2D
{
	public Vector2I size {private set; get;}

	protected Sprite2D sprite;

	public Building(Vector2I size)
	{
	}

	public abstract void GiveItem(Item item);
}
