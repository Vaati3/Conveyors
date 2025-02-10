using Godot;

public partial class Splitter : Building
{
    int outputIndex = 0;
    public Splitter(Vector2I pos, OutputCreatedEventHandler outputCreated) : base(pos, "Splitter", outputCreated)
    {
        sprite.Position = new Vector2(Map.tilesize * 0.5f, Map.tilesize * 0.5f);

        size = Vector2I.One * 2;
        AddInput(new Vector2(-Map.tilesize, Map.tilesize), new Vector2I(0, 1));
        AddOutput(new Vector2I(1, 0));
        AddOutput(new Vector2I(1, 1));

        ((RectangleShape2D)area.GetChild<CollisionShape2D>(0).Shape).Size = new Vector2(128, 256);
        area.GetChild<CollisionShape2D>(0).Position = area.GetChild<CollisionShape2D>(0).Position + Vector2.Left * Map.tilesize * 0.5f; 

        area.AreaEntered += AreaEntered;
    }

    private void AreaEntered(Area2D other)
    {
        if (other.Owner is Item item)
        {
            if (output[outputIndex].output == BeltInput.None){
                item.QueueFree();
                return;
            }
            item.Position = output[outputIndex].pos * Map.tilesize;
            item.belt = output[outputIndex];
            outputIndex = outputIndex == 0 ? 1 : 0;
        }
    }

    public override void Pause(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}