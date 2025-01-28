using Godot;
using System;

public partial class Map : Node2D
{
	public override void _Ready()
	{
		Belt belt = new Belt(Direction.LeftBottom);
		belt.Position = new Vector2(64, 64);
		Belt belt1 = new Belt(Direction.TopRight);
		belt1.Position = new Vector2(64, 192);
		AddChild(belt);
		AddChild(belt1);
	}
}
