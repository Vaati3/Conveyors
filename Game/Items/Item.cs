using Godot;
using System;

public enum ItemType {
    Circle = 1,
    Triangle = 3,
    Square,
    Pentagon,
    Hexagon
}

public partial class Item : Node2D
{
    public ItemType type {get; private set;}
    public Vector2 direction;
    public float speed = 50;
	private Sprite2D sprite;
    private Area2D area;

    public bool isPaused;

	public Item(ItemType type)
	{
        this.type = type;
        direction = Vector2.Zero;

        sprite = new Sprite2D(){
            Scale = new Vector2(0.2f, 0.2f)
        };
        
        sprite.Texture = GD.Load<Texture2D>("res://Game/Items/" + type.ToString() + ".png");
        AddChild(sprite);

        area = new Area2D();
		AddChild(area);
		area.AddChild(new CollisionShape2D(){
			Shape = new RectangleShape2D() {
				Size = new Vector2(1, 1)
			}
		});

        area.Owner = this;
	}

    public override void _PhysicsProcess(double delta)
    {
        if (isPaused)
            return;
        Position = Position + (direction * speed * (float)delta);
        // Vector2I pos = new Vector2I((int)Position.X, (int)Position.Y);//change if item change size!
        Vector2I pos = new Vector2I((int)Math.Floor((Position.X + Map.tilesize / 2) / Map.tilesize), (int)Math.Floor((Position.Y + Map.tilesize / 2) / Map.tilesize));
        if (GetNodeAt(pos) == null)
            QueueFree();
    }

    public Building.GetNodeAtEventHandler GetNodeAt;
}
