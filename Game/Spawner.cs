using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node
{
    RandomNumberGenerator rng;
    Dictionary<Vector2I, Node2D> nodes;
    Timer timer;

    GameUi ui;
	Node itemLayer;
	Node buildingLayer;

    private delegate Vector2I GetLimits();
    private GetLimits getLimits;

    AnimatedSprite2D synchro;

    public Spawner(Dictionary<Vector2I, Node2D> nodes, Map map)
    {
        this.nodes = nodes;
        getLimits += map.GetLimits;
        ui = map.ui;
        synchro = map.synchro;

		itemLayer = map.GetNode<Node>("Items");
		buildingLayer = map.GetNode<Node>("Buildings");

        rng = new RandomNumberGenerator();
        timer = new Timer() {
            Autostart = true,
            OneShot = true,
            WaitTime = 2
        };
        timer.Timeout += CreateSourceAndShop;
        AddChild(timer);
    }

    private void CreateSourceAndShop()
    {
        int n = rng.RandiRange(1, 6);
        n = n == 2 ? 1 : n;
        ItemType type = (ItemType)n;
		
        CreateSource(type);
        CreateShop(type);

        if (timer.IsStopped())
        {
            timer.WaitTime += 60;
            timer.Start();
        }
    }

    private void CreateSource(ItemType type)
    {
        Vector2I? pos = GetRandomPos(Vector2I.One);
        if (pos == null)
            return;
        Source source = new Source(pos.Value, OutputCreated, type, itemLayer);
        ui.Pause += source.Pause;
		buildingLayer.AddChild(source);
    }

    private void CreateShop(ItemType type)
    {
        int rot = 90 * rng.RandiRange(0, 3);
        Vector2I size = rot == 0 || rot == 180 ? new Vector2I(3,2) : new Vector2I(2,3);
        Vector2I? pos = GetRandomPos(size);
        if (pos == null)
            return;
        Shop shop = new Shop(pos.Value, OutputCreated, type, rot);
        ui.Pause += shop.Pause;
        buildingLayer.AddChild(shop);
        AddBuilding(shop);
    }

    private void AddBuilding(Building building)
    {
        for (int x = building.pos.X; x < building.pos.X + building.size.X; x++)
        {
            for (int y = building.pos.Y; y < building.pos.Y + building.size.Y; y++)
            {
                nodes.Add(new Vector2I(x, y), building);
            }
        }
    }

    private bool CanPlace(Vector2I pos, Vector2I size)
    {
        for (int x = pos.X; x < pos.X + size.X; x++)
        {
            for (int y = pos.Y; y < pos.Y + size.Y; y++)
            {
                if (nodes.ContainsKey(new Vector2I(x, y)))
                    return false;
            }
        }
        return true;
    }

    private Vector2I? GetRandomPos(Vector2I size)
    {
        Vector2I limits = getLimits();
        Vector2I start = new Vector2I(rng.RandiRange(-limits.X, limits.X), rng.RandiRange(-limits.Y, limits.Y));
        Vector2I pos;

        for (pos.X = start.X; pos.X < limits.X; pos.X++)
        {
            for (pos.Y = start.Y; pos.Y < limits.Y; pos.Y++)
            {
                if (CanPlace(pos, size))
                {
                    return pos;
                }
            }
        }
        for (pos.X = -limits.X; pos.X < start.X; pos.X++)
        {
            for (pos.Y = -limits.Y; pos.Y < start.Y; pos.Y++)
            {
                if (CanPlace(pos, size))
                {
                    return pos;
                }
            }
        }
        
        return null;
    }

    public Node2D GetNodeAt(Vector2I pos)
	{
		if (nodes.ContainsKey(pos))
		{
			return nodes[pos];
		}
		return null;
	}

    public void OutputCreated(Belt belt)
    {
        nodes[belt.pos] = belt;

        ui.Pause += belt.Pause;
        belt.synchro = synchro;
    }
}