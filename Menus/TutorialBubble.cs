using Godot;

public partial class TutorialBubble : Control
{
    RichTextLabel label;
    Panel panel;

    int index = 0;
    static string[] texts = {
        "[center]Welcome to Conveyors\n\n\nTap to continue[/center]",
        "[center]The objective of the game is to deliver different type of items to their destination using conveyor belts\nSelect Belts with the [img]res://Menus/Textures/Belt.png[/img] button then place them on the map by tapping or dragging [/center]",
        "[center]Each source and destination can produce/receive only one type of items.\n A item value define its shape and colour from [img]res://Game/Items/Circle.png[/img] to [img]res://Game/Items/Hexagon.png[/img]\nIf a building counter overflow the game end[/center]",
        "[center]You can use buildings to adjust the shape of the items.\nPlace building by using the other buttons [img]res://Menus/Textures/Operator.png[/img] [img]res://Menus/Textures/Splitter.png[/img] [img]res://Menus/Textures/Merger.png[/img] [img]res://Menus/Textures/Teleporter.png[/img]\n Then connect them with belts[/center]",
        "[center]You are limited in the amount of belts and buildings you can use\nRecover belts and buildings using [img]res://Menus/Textures/Remove.png[/img]\n Earn more through regular rewards[/center]",
        "[center]Use [img]res://Menus/Textures/Pause.png[/img]/[img]res://Menus/Textures/Play.png[/img] to pause and resume the game\n\nTap to start[/center]"
    };

    public delegate void TogglePauseEventHandler();
    private TogglePauseEventHandler TogglePause;

    public TutorialBubble(TogglePauseEventHandler togglePause)
    {
        TogglePause = togglePause;
        TogglePause();
        SetAnchorsPreset(LayoutPreset.FullRect);
        GuiInput += input;

        panel = new Panel() {
            Size = new Vector2(700, 120),
            Position = new Vector2(-350, 115),
            Theme = GD.Load<Theme>("res://Menus/Themes/TutorialPanel.tres")
        };
        panel.SetAnchorsPreset(LayoutPreset.CenterTop);
        AddChild(panel);
        
        label = new RichTextLabel()
        {
            Size = new Vector2(700, 110),
            Position = new Vector2(0, 5),
            BbcodeEnabled = true,
            Text = texts[index]
        };
        label.Set("theme_override_fonts/normal_font", GD.Load<Font>("res://Menus/Themes/Audiowide-Regular.ttf"));
        panel.AddChild(label);
    }

    private void input(InputEvent @event)
    {
        if (@event is InputEventMouse mouse)
        {
            if (mouse.ButtonMask == MouseButtonMask.Left && mouse.IsPressed())
            {
                index++;

                if (index >= texts.Length)
                {
                    TogglePause();
                    QueueFree();
                    return;
                }
                label.Text = texts[index];
            }
        }
    }
}