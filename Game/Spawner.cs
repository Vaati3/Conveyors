using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node
{
    Dictionary<Vector2I, Node2D> nodes;

	Node itemLayer;
	Node buildingLayer;

    private delegate Vector2I GetLimits();
    private GetLimits getLimits;

    RandomNumberGenerator rng;
    Timer timer;

    public Spawner(Dictionary<Vector2I, Node2D> nodes, Map map)
    {
        this.nodes = nodes;
        getLimits += map.GetLimits;
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
    
        Vector2I pos = GetRandomPos();
        Source source = new Source(pos, type, ItemCreated);
		source.GetNodeAt += GetNodeAt;
		nodes.Add(pos, source);
		buildingLayer.AddChild(source);

        pos = GetRandomPos();
        Shop shop = new Shop(pos, type);
		nodes.Add(pos, shop);
		buildingLayer.AddChild(shop);

        if (timer.IsStopped())
        {
            timer.WaitTime += 60;
            timer.Start();
        }
    }

    private Vector2I GetRandomPos()
    {
        Vector2I limits = getLimits() - Vector2I.One;
        Vector2I pos = new Vector2I(rng.RandiRange(-limits.X, limits.X), rng.RandiRange(-limits.Y, limits.Y));

        while (GetNodeAt(pos) != null)
        {
            pos.X = rng.RandiRange(-limits.X, limits.X);
            pos.Y = rng.RandiRange(-limits.Y, limits.Y);
        }
        return pos;
    }


    public void ItemCreated(Item item)
	{
		itemLayer.AddChild(item);
	}

    public Node2D GetNodeAt(Vector2I pos)
	{
		if (nodes.ContainsKey(pos))
		{
			return nodes[pos];
		}
		return null;
	}
}