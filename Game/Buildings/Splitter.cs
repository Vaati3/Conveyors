using Godot;

public partial class Splitter : Building
{
    int outputIndex = 0;
    Button buttonTop;
    ItemType filterTop = ItemType.None;
    Button buttonBottom;
    ItemType filterBottom= ItemType.None;
    public Splitter(Vector2I pos, InternalBeltCreatedEventHandler outputCreated) : base(pos, "Splitter", outputCreated)
    {
        mode = PlaceMode.Splitter;
        sprite.Position = new Vector2(Map.tilesize * 0.5f, Map.tilesize * 0.5f);

        size = new Vector2I(2, 2);
        AddInput(new Vector2I(0, 1), BeltInput.Bottom);
        AddOutput(new Vector2I(1, 0));
        AddOutput(new Vector2I(1, 1));

        ((RectangleShape2D)area.GetChild<CollisionShape2D>(0).Shape).Size = new Vector2(Map.tilesize, Map.tilesize*2);
        area.GetChild<CollisionShape2D>(0).Position = area.GetChild<CollisionShape2D>(0).Position + Vector2.Left * Map.tilesize * 0.5f; 

        area.AreaEntered += AreaEntered;

        buttonTop = new Button(){
            Size = Vector2.One * 60,
            Position = new Vector2(18, -95),
            MouseFilter = Control.MouseFilterEnum.Pass
            //add theme
        };
        sprite.AddChild(buttonTop);
        buttonTop.Pressed += ButtonTopPressed;

        buttonBottom = new Button(){
            Size = Vector2.One * 60,
            Position = new Vector2(18, 45),
            MouseFilter = Control.MouseFilterEnum.Pass
            //add theme
        };
        sprite.AddChild(buttonBottom);
        buttonBottom.Pressed += ButtonBottomPressed;
    }

    private void AreaEntered(Area2D other)
    {
        if (other.Owner is Item item)
        {
            if (filterTop != ItemType.None || filterBottom != ItemType.None)
            {
                if (item.type == filterTop)
                {
                    if (output[0].output == BeltInput.None)
                    {
                        item.QueueFree();
                        return;
                    }
                    item.Position = output[0].pos * Map.tilesize;
                    item.belt = output[0];
                    return;
                }
                if (item.type == filterBottom)
                {
                    if (output[1].output == BeltInput.None)
                    {
                        item.QueueFree();
                        return;
                    }
                    item.Position = output[1].pos * Map.tilesize;
                    item.belt = output[1];
                    return;
                }
                if (filterTop == ItemType.None)
                {
                    item.Position = output[0].pos * Map.tilesize;
                    item.belt = output[0];
                    return;
                }
                if (filterBottom == ItemType.None)
                {
                    item.Position = output[1].pos * Map.tilesize;
                    item.belt = output[1];
                    return;
                }
                item.QueueFree();
                return;
            }

            if (output[outputIndex].output == BeltInput.None)
            {
                outputIndex = outputIndex == 0 ? 1 : 0;
                if (output[outputIndex].output == BeltInput.None)
                {
                    item.QueueFree();
                    return;
                }
            }
            item.Position = output[outputIndex].pos * Map.tilesize;
            item.belt = output[outputIndex];
            outputIndex = outputIndex == 0 ? 1 : 0;
        }
    }

    public void ButtonTopPressed()
    {
        if (filterTop == ItemType.Hexagon)
        {
            filterTop = ItemType.None;
            buttonTop.Icon = null;
            return;
        }
        filterTop += filterTop == ItemType.None || filterTop == ItemType.Circle ? 2 : 1;
        buttonTop.Icon = GD.Load<Texture2D>("res://Game/Items/" + filterTop.ToString() + ".png");;
    }

    public void ButtonBottomPressed()
    {
        if (filterBottom == ItemType.Hexagon)
        {
            filterBottom = ItemType.None;
            buttonBottom.Icon = null;
            return;
        }
        filterBottom += filterBottom == ItemType.None || filterBottom == ItemType.Circle ? 2 : 1;
        buttonBottom.Icon = GD.Load<Texture2D>("res://Game/Items/" + filterBottom.ToString() + ".png");;
    }

    public override void Pause(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    public override void Rotate()
    {
        sprite.RotationDegrees += 90;
    }
}