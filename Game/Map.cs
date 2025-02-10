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
	AnimatedSprite2D synchro;
	Belt previousBelt = null;

	ColorRect background;
	Camera2D camera;
	const float zoomOutSpeed = 0.001f;
	

	private void PlaceBelt(Vector2I pos)
	{	
		if (!IsInLimits(pos))
			return;
		if (!nodes.ContainsKey(pos))
		{
			Belt belt = new Belt(pos, synchro, previousBelt);
			ui.Pause += belt.Pause;
			if (ui.isPaused)
				belt.Pause(true);
			nodes.Add(pos, belt);
			beltLayer.AddChild(belt);
			previousBelt = belt;
		}
	}

	public override void _Ready()
	{
		ui = GetNode<GameUi>("GameUi");
		nodes = new Dictionary<Vector2I, Node2D>();

		camera = GetNode<Camera2D>("Camera");
		background = GetNode<ColorRect>("Background");
		beltLayer = GetNode<Node>("Belts");
		synchro = GetNode<AnimatedSprite2D>("Synchro");
		synchro.Play();
		
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

	public static Vector2I GetTilePos(Vector2 position)
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

    public override void _UnhandledInput(InputEvent @event)
    {
		Vector2 mousePos = GetGlobalMousePosition();
		Vector2I pos =  GetTilePos(mousePos);
        if ( @event is InputEventMouseMotion)
		{
			if (Input.IsActionPressed("Click") && ui.mode == PlaceMode.Belt)
			{
				if (spawner.GetNodeAt(pos) is Belt belt && belt.pos != previousBelt.pos)
				{
					belt.Connect(previousBelt);
					previousBelt = belt;
				} else
					PlaceBelt(GetTilePos(mousePos));
			}
		} 
		else if (@event is InputEventMouseButton button)
		{
			if (button.ButtonIndex == MouseButton.Left)
			{
				if (ui.mode == PlaceMode.Remove)
				{
					if (spawner.GetNodeAt(pos) is Belt belt)
					{
						belt.Remove();
						nodes.Remove(pos);
					}
				}
				if (ui.mode == PlaceMode.Belt)
				{
					if (button.Pressed)
					{
						if (spawner.GetNodeAt(pos) is Belt belt)
							previousBelt = belt;
						else
							PlaceBelt(GetTilePos(mousePos));
					}
					else
						previousBelt = null;
				}
			}
		}
    }
}
