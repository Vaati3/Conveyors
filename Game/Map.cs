using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Node2D
{
	public static int tilesize = 128;
	public GameUi ui {get; private set;}
	public Spawner spawner {get; private set;}
	Dictionary<Vector2I, Node2D> nodes;

	Node beltLayer;
	Belt synchroBelt = null;
	Belt previousBelt = null;

	ColorRect background;
	Camera2D camera;
	const float zoomOutSpeed = 0.0005f;
	
	private void PlaceBelt(Vector2I pos, Vector2I dir)
	{	
		if (!IsInLimits(pos))
			return;
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

		camera = GetNode<Camera2D>("Camera");
		background = GetNode<ColorRect>("Background");
		beltLayer = GetNode<Node>("Belts");
		
		spawner = new Spawner(nodes, this);
		AddChild(spawner);
	}

	public Vector2I GetLimits()
	{
		return new Vector2I((int)MathF.Floor(background.Size.X / 2 / tilesize), (int)MathF.Floor(background.Size.Y / 2 / tilesize));
	}

	private bool IsInLimits(Vector2I pos)
	{
		Vector2I limits = GetLimits();
		
		if (Math.Abs(pos.X) > limits.X || Math.Abs(pos.Y) > limits.Y)
			return false;
		return true;
	}

	private Vector2I GetTilePos(Vector2 position)
	{
		return new Vector2I((int)Math.Floor((position.X + tilesize / 2) / tilesize), (int)Math.Floor((position.Y + tilesize / 2) / tilesize));
	}

	public override void _Process(double delta)
	{
		float value = camera.Zoom.X - zoomOutSpeed * (float)delta;
		camera.Zoom = new Vector2(value, value);
		background.Size = GetViewportRect().Size/camera.Zoom;
		background.Position = -background.Size/2;
	}

    public override void _Input(InputEvent @event)
    {
		Vector2 mousePos = GetGlobalMousePosition();
        if (ui.mode == PlaceMode.Belt && @event is InputEventMouseMotion motion)
		{
			if (Input.IsActionPressed("Click"))
			{
				Vector2I pos = GetTilePos(mousePos);
				Vector2I dir = Vector2I.Zero; 
				if (MathF.Abs(motion.Velocity.X) > MathF.Abs(motion.Velocity.Y))
					dir.X = motion.Velocity.X > 0 ? 1 : -1;
				else
					dir.Y = motion.Velocity.Y > 0 ? 1 : -1;
				
				PlaceBelt(pos, dir);
			} else {
				previousBelt = null;
			}
		} else if (ui.mode == PlaceMode.Remove && @event is InputEventMouseButton button)
		{
			if (button.ButtonIndex == MouseButton.Left)
			{
				Vector2I pos =  GetTilePos(mousePos);
				if (spawner.GetNodeAt(pos) is Belt belt)
				{
					belt.QueueFree();
					nodes.Remove(pos);
				}
			}
		}
    }
}
