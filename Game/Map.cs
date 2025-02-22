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
	public AnimatedSprite2D synchro {get; private set;}
	Belt previousBelt = null;

	ColorRect background;
	Camera2D camera;
	const float zoomOutSpeed = 0.004f;
	const float startZoom = 0.85f;
	const float endZoom = 0.35f;
	double weight = 0;

	List<Teleporter> teleporters;
	
	private void PlaceBelt(Vector2I pos)
	{	
		if (!IsInLimits(pos))
		{
			previousBelt = null;
			return;
		}
		
		if (!nodes.ContainsKey(pos) && ui.GetCount(PlaceMode.Belt) > 0)
		{
			Belt belt = new Belt(pos, synchro, previousBelt, ui.soundManager);
			ui.Pause += belt.Pause;
			if (ui.isPaused)
				belt.Pause(true);
			nodes.Add(pos, belt);
			beltLayer.AddChild(belt);
			previousBelt = belt;
			ui.ChangeCount(PlaceMode.Belt, -1);
		} else if (!nodes.ContainsKey(pos)) {
			previousBelt = null;
		}
	}

	private void PlaceSplitter(Vector2I pos)
	{
		if (ui.GetCount(PlaceMode.Splitter) > 0 && spawner.CanPlace(pos, Vector2I.One * 2, GetLimits()))
		{
        	Splitter splitter = new Splitter(pos, spawner.InternalBeltCreated);
        	ui.Pause += splitter.Pause;
			spawner.AddBuilding(splitter);
			ui.ChangeCount(PlaceMode.Splitter, -1);
		}
	}

	private void PlaceOperator(Vector2I pos)
	{
		if (ui.GetCount(PlaceMode.Operator) > 0 && spawner.CanPlace(pos, Vector2I.One, GetLimits()))
		{
			Operator @operator = new Operator(pos, spawner.InternalBeltCreated);
        	ui.Pause += @operator.Pause;
			spawner.AddBuilding(@operator);
			ui.ChangeCount(PlaceMode.Operator, -1);
		}
	}

	private void PlaceMerger(Vector2I pos)
	{
		if (ui.GetCount(PlaceMode.Merger) > 0 && spawner.CanPlace(pos, Vector2I.One, GetLimits()))
		{
			Merger merger = new Merger(pos, spawner.InternalBeltCreated);
        	ui.Pause += merger.Pause;
			spawner.AddBuilding(merger);
			ui.ChangeCount(PlaceMode.Merger, -1);
		}
	}

	private void PlaceTeleporter(Vector2I pos)
	{
		if (ui.GetCount(PlaceMode.Teleporter) > 0 && spawner.CanPlace(pos, Vector2I.One, GetLimits()))
		{
			Teleporter teleporter = new Teleporter(pos, spawner.InternalBeltCreated, teleporters);
        	ui.Pause += teleporter.Pause;
			spawner.AddBuilding(teleporter);
			ui.ChangeCount(PlaceMode.Teleporter, -1);
			teleporters.Add(teleporter);
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

		teleporters = new List<Teleporter>();
	}

	public void SetupUI(int highScore, GameUi.QuitToMenuEventHandler quitToMenu, CheckBox tutoCheck)
	{
		ui.highScore = highScore;
		ui.QuitToMenu += quitToMenu;
		if (tutoCheck.ButtonPressed)
			ui.AddChild(new TutorialBubble(ui.TogglePause, tutoCheck));
	}

	public Vector2I GetLimits()
	{
		return new Vector2I((int)MathF.Floor((background.Size.X-tilesize) / 2 / tilesize) , (int)MathF.Floor((background.Size.Y-tilesize) / 2 / tilesize));
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
		if (ui.isPaused)
			return;
		weight += zoomOutSpeed * (float)delta;
		float value = (float)Mathf.Lerp(startZoom, endZoom, weight);
		camera.Zoom = new Vector2(value, value);

		background.Size = GetViewportRect().Size/camera.Zoom;
		background.Position = -background.Size/2;
	}

	private void Remove(Vector2I pos)
	{
		if (spawner.GetNodeAt(pos) is Belt belt)
		{
			if (belt.building != null)
			{
				if (!belt.building.isRemovable)
					return;
				ui.ChangeCount(belt.building.mode, 1);
				spawner.RemoveBuilding(belt.building);
				return;
			}
			belt.Remove();
			nodes.Remove(pos);
			ui.ChangeCount(PlaceMode.Belt, 1);
			return;
		}
		if (spawner.GetNodeAt(pos) is Building building)
		{
			if (!building.isRemovable)
				return;
			ui.ChangeCount(building.mode, 1);
			spawner.RemoveBuilding(building);
		}
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		Vector2 mousePos = GetGlobalMousePosition();
		Vector2I pos =  GetTilePos(mousePos);
        if ( @event is InputEventMouseMotion)
		{
			if (Input.IsActionPressed("Click") && ui.mode == PlaceMode.Belt)
			{
				if (spawner.GetNodeAt(pos) is Belt belt && previousBelt != null && belt.pos != previousBelt.pos)
				{
					belt.Connect(previousBelt);
					previousBelt = belt;
				} else
					PlaceBelt(GetTilePos(mousePos));
			}
			if (Input.IsActionPressed("Click") && ui.mode == PlaceMode.Remove)
			{
				Remove(pos);	
			}
		} 
		else if (@event is InputEventMouseButton button)
		{
			if (button.ButtonIndex == MouseButton.Left)
			{
				if (button.DoubleClick)
				{
					if (spawner.GetNodeAt(pos) is Belt belt)
					{
						if (belt.building != null && belt.building.isRemovable)
						{
							spawner.RotateBuilding(belt.building);
						}
						return;
					}
					if (spawner.GetNodeAt(pos) is Building building)
					{
						if (building.isRemovable)
						{
							spawner.RotateBuilding(building);
						}
						return;
					}
				}
				switch(ui.mode)
				{
					case PlaceMode.Remove:
						Remove(pos);
						break;
					case PlaceMode.Belt:
						if (button.Pressed)
						{
							if (spawner.GetNodeAt(pos) is Belt belt)
								previousBelt = belt;
							else
								PlaceBelt(GetTilePos(mousePos));
						} else
							previousBelt = null;
						break;
					case PlaceMode.Splitter:
						if (button.Pressed)
							PlaceSplitter(pos);
						break;
					case PlaceMode.Operator:
						if (button.Pressed)
							PlaceOperator(pos);
						break;
					case PlaceMode.Merger:
						if (button.Pressed)
							PlaceMerger(pos);
						break;
					case PlaceMode.Teleporter:
						if (button.Pressed)
							PlaceTeleporter(pos);
						break;

				}
			}
		}
    }
}
