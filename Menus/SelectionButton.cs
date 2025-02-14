using Godot;

public partial class SelectionButton : Control
{
    [Export] PlaceMode mode;
    bool selected = false;
    TextureRect icon;
    Button button;

    [Export] public int count = 0; 
    Label countLabel;

    public override void _Ready()
    {
        icon = GetNode<TextureRect>("Icon");
        button = GetNode<Button>("Button");
        countLabel = GetNode<Label>("Count");

        icon.Texture = GD.Load<Texture2D>("res://Menus/Textures/" + mode.ToString() + ".png");

        if (mode == PlaceMode.Remove)
            countLabel.Visible = false;
        else
            UpdateCount(0);

        if (mode == PlaceMode.Belt)
            button.Disabled = true;
    }

    public void UpdateCount(int value)
    {
        count += value;
        count = count < 0 ? 0 : count;
        countLabel.Text = count.ToString();
    }
    
    public void Unselect()
    {
        button.Disabled = false;
        selected = false;
    }

    private void Pressed()
    {
        button.Disabled = true;
        EmitSignal(nameof(ModeSelected), (int)mode);
    }


    [Signal] public delegate void ModeSelectedEventHandler(PlaceMode mode);
}
