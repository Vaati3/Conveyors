using Godot;
using System;

public enum ItemType {
    None = -1,
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

    public Belt belt;
	private Sprite2D sprite;
    private Area2D area;

    public bool isPaused;

	public Item(ItemType type)
	{
        this.type = type;
        direction = Vector2.Zero;

        sprite = new Sprite2D();
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

    private Vector2 GetDirection()
    {
        switch(belt.output)
		{
			case BeltInput.Bottom:
				return Vector2.Down;
			case BeltInput.Left:
				return Vector2.Left;
			case BeltInput.Right:
				return Vector2.Right;
			case BeltInput.Top:
				return Vector2.Up;
		}
		return Vector2.Zero;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isPaused)
            return;
        Position = Position + (GetDirection() * belt.speed * (float)delta);
    }
}
