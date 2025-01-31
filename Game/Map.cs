using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Node2D
{
	public static int tilesize = 128;
	Dictionary<Vector2I, Node2D> nodes;
	Belt synchroBelt = null;
	Belt previousBelt = null;

	private void CreateBelt(Vector2I pos, Vector2I dir)
	{	
		if (!nodes.ContainsKey(pos))
		{
			Belt belt = new Belt(pos, Belt.GetBeltDirection(pos, dir, previousBelt), synchroBelt);
			// belt.SendItem += SendItem;
			nodes.Add(pos, belt);
			AddChild(belt);
			previousBelt = belt;
			if (synchroBelt == null)
				synchroBelt = belt;
		}
	}

	public override void _Ready()
	{
		nodes = new Dictionary<Vector2I, Node2D>();

		//replace with random placement. create new class?
		Source source = new Source(new Vector2I(2, 2), ItemType.Circle);
		source.GetNodeAt += GetNodeAt;

		nodes.Add(new Vector2I(2, 2), source);
		AddChild(source);
	}

	public Node2D GetNodeAt(Vector2I pos)
	{
		if (nodes.ContainsKey(pos))
		{
			return nodes[pos];
		}
		return null;
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
		{
			if (Input.IsActionPressed("Click"))
			{
				Vector2I pos = new Vector2I((int)Math.Floor(motion.Position.X / tilesize), (int)Math.Floor(motion.Position.Y / tilesize));
				Vector2I dir = Vector2I.Zero; 
				if (MathF.Abs(motion.Velocity.X) > MathF.Abs(motion.Velocity.Y))
					dir.X = motion.Velocity.X > 0 ? 1 : -1;
				else
					dir.Y = motion.Velocity.Y > 0 ? 1 : -1;
				
				CreateBelt(pos, dir);
			} else {
				previousBelt = null;
			}
		}
    }
}
