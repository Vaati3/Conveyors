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
    private Vector2 dir = Vector2.Zero;
	private Sprite2D sprite;
    private Area2D area;

    public bool isPaused;
    public bool isStoped = false;

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

    private void SetDirection()
    {
        switch(belt.output)
		{
			case BeltInput.Bottom:
				dir = Vector2.Down;
                return;
			case BeltInput.Left:
				dir = Vector2.Left;
                return;
			case BeltInput.Right:
				dir = Vector2.Right;
                return;
			case BeltInput.Top:
				dir = Vector2.Up;
                return;
		}
		dir = Vector2.Zero;
    }


    public void SetType(ItemType type)
    {
        this.type = type;

        sprite.Texture = GD.Load<Texture2D>("res://Game/Items/" + type.ToString() + ".png");
    }

    private bool CheckDirection()
    {
        if (dir.X != 0)
        {
            if (dir.X > 0)
                return Position.X >= belt.Position.X;
            return Position.X <= belt.Position.X;
        }
        if (dir.Y > 0)
            return Position.Y >= belt.Position.Y;
        return Position.Y <= belt.Position.Y;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (belt == null)
        {
            QueueFree();
            return;
        }
        float dist = MathF.Sqrt(MathF.Pow(belt.Position.X - Position.X, 2) + MathF.Pow(belt.Position.Y - Position.Y, 2));
        if (dist > Map.tilesize)
        {
            QueueFree();
            return;
        }
        if (isPaused || isStoped || belt.output == BeltInput.None)
            return;

        if (CheckDirection())
            SetDirection();
        Position = Position + (dir * belt.speed * (float)delta);
    }
}
