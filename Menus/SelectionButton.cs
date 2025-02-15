using System;
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

        if (mode == PlaceMode.Belt){
            selected = true;
            button.Disabled = true;
        }
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
        weight = 0; 
    }

    [Signal] public delegate void ModeSelectedEventHandler(PlaceMode mode);
    private void Pressed()
    {
        button.Disabled = true;
        selected = true;
        weight = 0; 
        EmitSignal(nameof(ModeSelected), (int)mode);
    }

    float weight = 0;
    public override void _PhysicsProcess(double delta)
    {
        weight += (float)delta/5;

        float originAngle = selected ? MathF.PI / 4 : 0;
        float destAngle = selected ? MathF.PI / 4 : 0;

        button.Rotation = Mathf.LerpAngle(originAngle, destAngle, weight);
    }
}
