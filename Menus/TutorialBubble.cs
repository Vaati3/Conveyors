using Godot;

public partial class TutorialBubble : Control
{
    SoundManager soundManager = null;
    RichTextLabel label;
    Panel panel;

    int index = 0;
    static string[] texts = {
        "[center]Welcome to Conveyors\n\n\nTap to continue[/center]",
        "[center]The objective of the game is to deliver different type of items to their destination using conveyor belts\nSelect Belts with the [img]res://Menus/Textures/Belt.png[/img] button then place them on the map by tapping or dragging [/center]",
        "[center]Each source and destination can produce/receive only one type of items.\n A item value define its shape and colour from [img]res://Game/Items/Circle.png[/img] to [img]res://Game/Items/Hexagon.png[/img]\nIf a building counter decrease to zero the game end[/center]",
        "[center]You can use buildings to adjust the shape of the items.\nPlace building by using the other buttons [img]res://Menus/Textures/Operator.png[/img] [img]res://Menus/Textures/Splitter.png[/img] [img]res://Menus/Textures/Merger.png[/img] [img]res://Menus/Textures/Teleporter.png[/img]\n Then connect them with belts[/center]",
        "[center]You are limited in the amount of belts and buildings you can use\nBuilding inputs are in red and output in Blue\nRecover belts and buildings using [img]res://Menus/Textures/Remove.png[/img]\n Earn more through regular rewards[/center]",
        "[center]Rotate Building by double tapping\nUse [img]res://Menus/Textures/Pause.png[/img]/[img]res://Menus/Textures/Play.png[/img] to pause and resume the game\nTap to start[/center]"
    };

    public delegate void TogglePauseEventHandler();
    private TogglePauseEventHandler TogglePause;

    CheckBox tutoCheck;

    public TutorialBubble(TogglePauseEventHandler togglePause, CheckBox tutoCheck)
    {
        TogglePause = togglePause;
        this.tutoCheck = tutoCheck;
        TogglePause();
        SetAnchorsPreset(LayoutPreset.FullRect);
        GuiInput += input;

        panel = new Panel() {
            Size = new Vector2(700, 120),
            Position = new Vector2(-350, 115),
            Theme = GD.Load<Theme>("res://Menus/Themes/TutorialPanel.tres")
        };
        panel.SetAnchorsPreset(LayoutPreset.CenterTop);
        panel.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(panel);
        
        label = new RichTextLabel()
        {
            Size = new Vector2(700, 110),
            Position = new Vector2(0, 5),
            BbcodeEnabled = true,
            Text = texts[index]
        };
        label.MouseFilter = MouseFilterEnum.Ignore;
        label.Set("theme_override_fonts/normal_font", GD.Load<Font>("res://Menus/Themes/Audiowide-Regular.ttf"));
        panel.AddChild(label);
    }

    private void input(InputEvent @event)
    {
        if (@event is InputEventMouse mouse)
        {
            if (mouse.ButtonMask == MouseButtonMask.Left && mouse.IsPressed())
            {
                if (soundManager == null)
                    soundManager = GetNode<SoundManager>("/root/SoundManager");
                index++;
                soundManager.PlaySFX("Reward");
                if (index >= texts.Length)
                {
                    tutoCheck.ButtonPressed = false;
                    TogglePause();
                    QueueFree();
                    return;
                }
                label.Text = texts[index];
            }
        }
    }
}