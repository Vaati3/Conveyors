using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Node2D
{
	public static int tilesize = 128;
	public GameUi ui {get; private set;}
	Dictionary<Vector2I, Node2D> nodes;
	Belt synchroBelt = null;
	Belt previousBelt = null;

	Node beltLayer;
	Node itemLayer;
	Node buildingLayer;

	private void CreateBelt(Vector2I pos, Vector2I dir)
	{	
		if (!nodes.ContainsKey(pos))
		{
			Belt belt = new Belt(pos, Belt.GetBeltDirection(pos, dir, previousBelt), synchroBelt);
			// belt.SendItem += SendItem;
			nodes.Add(pos, belt);
			beltLayer.AddChild(belt);
			previousBelt = belt;
			if (synchroBelt == null)
				synchroBelt = belt;
		}
	}

	public override void _Ready()
	{
		ui = GetNode<GameUi>("GameUi");
		nodes = new Dictionary<Vector2I, Node2D>();

		beltLayer = GetNode<Node>("Belts");
		itemLayer = GetNode<Node>("Items");
		buildingLayer = GetNode<Node>("Buildings");

		//replace with random placement. create new class?
		Source source = new Source(new Vector2I(2, 1), ItemType.Circle, ItemCreated);
		source.GetNodeAt += GetNodeAt;
		nodes.Add(new Vector2I(2, 1), source);
		buildingLayer.AddChild(source);
		Shop shop = new Shop(new Vector2I(5, 4), ItemType.Circle);
		nodes.Add(new Vector2I(5, 4), shop);
		buildingLayer.AddChild(shop);
	}

	public Node2D GetNodeAt(Vector2I pos)
	{
		if (nodes.ContainsKey(pos))
		{
			return nodes[pos];
		}
		return null;
	}

	public void ItemCreated(Item item)
	{
		itemLayer.AddChild(item);
	}

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseButton button)
		{
			if (button.ButtonIndex == MouseButton.Left)
			{
				Vector2I pos = new Vector2I((int)Math.Floor(button.Position.X / tilesize), (int)Math.Floor(button.Position.Y / tilesize));

				if (GetNodeAt(pos) is Belt belt)
				{
					belt.QueueFree();
					nodes.Remove(pos);
				}
			}
		}
        if (@event is InputEventMouseMotion motion)
		{
			if (ui.mode == PlaceMode.Belt && Input.IsActionPressed("Click"))
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
