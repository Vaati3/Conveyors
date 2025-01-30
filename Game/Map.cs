using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Node2D
{
	public static int tilesize = 128;
	Dictionary<Vector2I, Belt> belts;
	Belt synchroBelt = null;
	Belt previousBelt = null;

	private void CreateBelt(Vector2I pos, Vector2I dir)
	{	
		if (!belts.ContainsKey(pos))
		{
			Belt belt = new Belt(pos, Belt.GetBeltDirection(pos, dir, previousBelt), synchroBelt);
			belts.Add(pos, belt);
			AddChild(belt);
			previousBelt = belt;
			if (synchroBelt == null)
				synchroBelt = belt;
		}
	}

	public override void _Ready()
	{
		belts = new Dictionary<Vector2I, Belt>();

		Source source = new Source(new Vector2I(2, 2), ItemType.Circle);
		source.SendItem += SendItem;
		AddChild(source);
	}

	public bool SendItem(Item item, Vector2I pos)
	{
		if (belts.ContainsKey(pos))
		{
			AddChild(item);
			if (!belts[pos].ReceiveItem(item))
				return false;
			return true;
		}
		return false;
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
		{
			if (Input.IsActionPressed("Click"))
			{
				Vector2I pos = new Vector2I((int)motion.Position.X / tilesize, (int)motion.Position.Y / tilesize);
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
