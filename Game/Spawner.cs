using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class Spawner : Node
{
    RandomNumberGenerator rng;
    Dictionary<Vector2I, Node2D> nodes;
    Timer sourceTimer;
    Timer shopTimer;

    GameUi ui;
    Node beltLayer;
	Node itemLayer;
	Node buildingLayer;

    private delegate Vector2I GetLimits();
    private GetLimits getLimits;

    AnimatedSprite2D synchro;

    ItemType lastType;

    public Spawner(Dictionary<Vector2I, Node2D> nodes, Map map)
    {
        this.nodes = nodes;
        getLimits += map.GetLimits;
        ui = map.ui;
        ui.Pause += Pause;
        synchro = map.synchro;

        beltLayer = map.GetNode<Node>("Belts");
		itemLayer = map.GetNode<Node>("Items");
		buildingLayer = map.GetNode<Node>("Buildings");

        rng = new RandomNumberGenerator();

        sourceTimer = new Timer() {
            Autostart = false,
            OneShot = true,
            WaitTime = 30
        };
        sourceTimer.Timeout += SourceTimeout;
        AddChild(sourceTimer);

        shopTimer = new Timer() {
            Autostart = false,
            OneShot = true,
            WaitTime = 60
        };
        shopTimer.Timeout += ShopTimeout; // Change to createshop or upgrade existing one or upgrade on source upgrade
        AddChild(shopTimer);

        int n = rng.RandiRange(1, 6);
        n = n == 2 ? 1 : n;
        lastType = (ItemType)n;
        CreateSource(lastType);
        CreateShop(lastType);
    }

    private void SourceTimeout()
    {
        int n = rng.RandiRange((int)lastType-1, (int)lastType+1);
        n = n == 2 || n < 1 ? 1 : n;
        lastType = (ItemType)n;
        CreateSource(lastType);
    }

    private void ShopTimeout()
    {
        int n = rng.RandiRange((int)lastType-1, (int)lastType+1);
        n = n == 2 || n < 1 ? 1 : n;
        lastType = (ItemType)n;
        CreateShop(lastType);
    }

    private void CreateSource(ItemType type)
    {
        ui.soundManager.PlaySFX("Source");
        Vector2I? pos = GetRandomPos(Vector2I.One);
        if (pos == null)
            return;
        Source source = new Source(pos.Value, InternalBeltCreated, type, itemLayer);
        ui.Pause += source.Pause;
		AddBuilding(source);
    }

    private void CreateShop(ItemType type)
    {
        ui.soundManager.PlaySFX("Shop");
        int rot = 90 * rng.RandiRange(0, 3);
        Vector2I size = rot == 0 || rot == 180 ? new Vector2I(3,2) : new Vector2I(2,3);
        Vector2I? pos = GetRandomPos(size);
        if (pos == null)
            return;
        Shop shop = new Shop(pos.Value, InternalBeltCreated, type, rot);
        ui.Pause += shop.Pause;
        shop.GameLost += ui.GameLost;
        AddBuilding(shop);
    }

    public void AddBuilding(Building building)
    {
        ui.soundManager.PlaySFX("Place");
        buildingLayer.AddChild(building);
        for (int x = building.pos.X; x < building.pos.X + building.size.X; x++)
        {
            for (int y = building.pos.Y; y < building.pos.Y + building.size.Y; y++)
            {
                foreach(Belt belt in building.output)
                {
                    if (belt.pos.X == x && belt.pos.Y == y)
                        nodes.Add(belt.pos, belt);
                }
                foreach(Belt belt in building.input)
                {
                    if (belt.pos.X == x && belt.pos.Y == y)
                        nodes.Add(belt.pos, belt);
                }
                if (!nodes.ContainsKey(new Vector2I(x, y)))
                    nodes.Add(new Vector2I(x, y), building);
            }
        }
    }

    public void RemoveBuilding(Building building)
    {
        for (int x = building.pos.X; x < building.pos.X + building.size.X; x++)
        {
            for (int y = building.pos.Y; y < building.pos.Y + building.size.Y; y++)
            {
                if (nodes[new Vector2I(x, y)] is Belt belt)
                    belt.Remove();
                nodes.Remove(new Vector2I(x,y));
            }
        }
        building.QueueFree();
    }

    public void RotateBuilding(Building building)
    {
        Vector2 center = new Vector2(building.pos.X + (((float)building.size.X-1)/2), building.pos.Y + (((float)building.size.Y-1)/2));
        
        List<Belt> belts = new List<Belt>();
        List<Vector2I> parts = new List<Vector2I>();
        for (int x = building.pos.X; x < building.pos.X + building.size.X; x++)
        {
            for (int y = building.pos.Y; y < building.pos.Y + building.size.Y; y++)
            {
                Vector2I pos;
                pos.X = (int)((x - center.X) * MathF.Cos(MathF.PI/2) - (y - center.Y) * MathF.Sin(MathF.PI/2) + center.X);
                pos.Y = (int)((x - center.X) * MathF.Sin(MathF.PI/2) + (y - center.Y) * MathF.Cos(MathF.PI/2) + center.Y);

                Node node = GetNodeAt(new Vector2I(x, y));
                if (node is Belt belt)
                {
                    belt.ChangePos(pos);
                    belts.Add(belt);
                } else {
                    parts.Add(pos);
                }
                nodes[new Vector2I(x, y)] = null;
            }
        }
        foreach(Belt belt in belts)
            nodes[belt.pos] = belt;
        foreach(Vector2I pos in parts)
            nodes[pos] = building;
        building.Rotate();
    }

    public bool CanPlace(Vector2I pos, Vector2I size)
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
        Vector2I limits = getLimits() - Vector2I.One;
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

    public void InternalBeltCreated(Belt belt)
    {
        ui.Pause += belt.Pause;
        belt.synchro = synchro;
        beltLayer.AddChild(belt);
    }

    private void Pause(bool isPaused)
    {
        if (isPaused)
            synchro.Pause();
        else
            synchro.Play();
        
        sourceTimer.Paused = isPaused;
        shopTimer.Paused = isPaused;
    }
}