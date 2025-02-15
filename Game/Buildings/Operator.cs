using Godot;

public partial class Operator : Building
{

    int operation = 1;
    Button button;
    public Operator(Vector2I pos, InternalBeltCreatedEventHandler outputCreated) : base(pos, "Operator", outputCreated)
    {
        mode = PlaceMode.Operator;
        AddOutput(Vector2I.Zero);

        area.AreaEntered += AreaEntered;

        button = new Button(){
            Size = Vector2.One * 60,
            Text = "+ 1",
            Position = Vector2.One * -32,
            MouseFilter = Control.MouseFilterEnum.Pass
            //add theme
        };
        sprite.AddChild(button);
        button.Pressed += ButtonPressed;
        button.Set("theme_override_fonts/font", GD.Load<Font>("res://Menus/Themes/Audiowide-Regular.ttf"));
        button.Set("theme_override_font_sizes/font_size", 50);
    }

    private void AreaEntered(Area2D other)
    {
        if (other.Owner is Item item)
        {
            ItemType newType = item.type;
            if (operation > 0)
            {
                newType += newType == ItemType.None || newType == ItemType.Circle ? 2 : 1;
                newType = (int)newType > 6 ? ItemType.Circle : newType;
                item.SetType(newType);
                return;
            }
            newType--;
            newType = (int)newType < 1 || (int)newType == 2 ? ItemType.Circle : newType;
            item.SetType(newType);
        }
    }

    private void ButtonPressed()
    {
        operation = operation * -1;

        button.Text = operation > 0 ? "+ 1" : "- 1";
    }

    public override void Pause(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
