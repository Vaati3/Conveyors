using System.Collections.Generic;
using Godot;

public partial class Merger : Building
{
    char operatorMode = '+';
    Button button;

    Queue<Item> items1;
    Queue<Item> items2;

    const int maxStorage = 5;
    public Merger(Vector2I pos, InternalBeltCreatedEventHandler outputCreated) : base(pos, "Merger", outputCreated)
    {
        items1 = new Queue<Item>();
        items2 = new Queue<Item>();
        sprite.Position = new Vector2(Map.tilesize * 0.5f, Map.tilesize * 0.5f);

        area.AreaEntered += AreaEntered;

        button = new Button(){
            Size = Vector2.One * 60,
            Text = " + ",
            Position = Vector2.One * 32,
            MouseFilter = Control.MouseFilterEnum.Pass
            //add theme
        };
        AddChild(button);
        button.Pressed += ButtonPressed;
        button.Set("theme_override_fonts/font", GD.Load<Font>("res://Menus/Themes/Audiowide-Regular.ttf"));
        button.Set("theme_override_font_sizes/font_size", 50);

        size = Vector2I.One * 2;
        AddInput(new Vector2I(0, 0), BeltInput.Right);
        AddInput(new Vector2I(0, 1), BeltInput.Top);
        AddOutput(new Vector2I(1, 1));
    }

    private void AreaEntered(Area2D other)
    {
        if (other.Owner is Item item)
        {
            item.isStoped = true;
            if (input[0].items.Contains(item))
            {
                items1.Enqueue(item);
                if (items1.Count > maxStorage)
                    items1.Dequeue().QueueFree();
            } else {
                items2.Enqueue(item);
                if (items2.Count > maxStorage)
                    items2.Dequeue().QueueFree();
            }

            if (items1.Count > 0 && items2.Count > 0)
            {
                Item item1 = items1.Dequeue();
                Item item2 = items2.Dequeue();
                if (output[0].output == BeltInput.None)
                {
                    item1.QueueFree();
                    item2.QueueFree();
                    return;
                }
                Vector2 dir = output[0].output == BeltInput.Bottom || output[0].output == BeltInput.Top ? Vector2.Down : Vector2.Right;
                ItemType type = GetNewType(item1.type, item2.type);

                item1.SetType(type);
                item2.SetType(type);
                item1.Position = output[0].pos * Map.tilesize + (dir * 26);
                item1.belt = output[0];
                item2.Position = output[0].pos * Map.tilesize - (dir * 26);
                item2.belt = output[0];
                item1.isStoped = false;
                item2.isStoped = false;
            }
        }
    }

    private ItemType GetNewType(ItemType type1, ItemType type2)
    {
        int newType = 0;
        switch(operatorMode)
        {
            case '+':
                newType =  (int)type1 + (int)type2;
                break;
            case '-':
                newType =  (int)type1 - (int)type2;
                break;
            case '*':
                newType =  (int)type1 * (int)type2;
                break;
            case '/':
                newType =  (int)type1 / (int)type2;
                break;
        }
        newType = newType == 2 ? 3 : newType;
        newType = newType < 0 ? 1 : newType;
        newType = newType > 6 ? 6 : newType;

        return (ItemType)newType;
    }

    private void ButtonPressed()
    {
        switch(operatorMode)
        {
            case '+':
                operatorMode = '-';
                break;
            case '-':
                operatorMode = '*';
                break;
            case '*':
                operatorMode = '/';
                break;
            case '/':
                operatorMode = '+';
                break;
        }
        button.Text = " " + operatorMode + " ";
    }

    public override void Pause(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}