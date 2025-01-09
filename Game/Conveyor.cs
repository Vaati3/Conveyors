using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Conveyor : Node2D
{
	public Vector2I direction {private set; get;}
	protected Sprite2D sprite;
    protected Queue<Item> items;

	public Conveyor(Vector2I size)
	{
	}

    public abstract void MoveItems();

    public abstract void GiveItem(Item item);
}
