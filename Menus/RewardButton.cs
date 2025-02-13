using Godot;

public partial class RewardButton : Button
{
    PlaceMode mode;
    int buildingAmount;
    int beltAmount;

    Label beltLabel;

    Control buildingBox;
    TextureRect buildingIcon;
    Label buildingLabel;
    SoundManager soundManager;

    public override void _Ready()
    {
        beltLabel = GetNode<Label>("Rewards/Bets/Label");

        buildingBox = GetNode<Control>("Rewards/Building");
        buildingIcon = GetNode<TextureRect>("Rewards/Building/Texture");
        buildingLabel = GetNode<Label>("Rewards/Building/Label");

        soundManager = GetNode<SoundManager>("/root/SoundManager");

        Pressed += RewardChosen;
    }

    public void Update(PlaceMode mode)
    {
        this.mode = mode;
        buildingBox.Visible = true;
        switch(mode) {
            case PlaceMode.Belt : case PlaceMode.Remove:
                beltAmount = 30;
                buildingAmount = 0;
                buildingBox.Visible = false;
                break;
            case PlaceMode.Splitter:
                beltAmount = 10;
                buildingAmount = 1;
                buildingIcon.Texture = GD.Load<Texture2D>("res://Menus/Textures/Splitter.png");
                buildingLabel.Text = buildingAmount.ToString();
                break;
            case PlaceMode.Operator:
                beltAmount = 15;
                buildingAmount = 2;
                buildingIcon.Texture = null;
                buildingLabel.Text = "+/- 1\n" + buildingAmount.ToString();//create icon
                break;
        }

        beltLabel.Text = beltAmount.ToString();
    }

    public delegate void RewardSelectedEventHandler(int beltAmount, PlaceMode building, int buildingAmount);

    public RewardSelectedEventHandler RewardSelected;

    private void RewardChosen()
    {
        RewardSelected(beltAmount, mode, buildingAmount);
        soundManager.PlaySFX("Button");
    }
}