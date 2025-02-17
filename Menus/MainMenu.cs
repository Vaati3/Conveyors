using Godot;
using System;

public partial class MainMenu : Control
{
	Map map;
	SoundManager soundManager;

	Panel optionsMenu;

	VolumeSlider[] sliders;

	int bestScore;
	Label scoreLabel;
	const string saveFile = "user://save.save";

    public override void _Ready()
    {
        soundManager = GetNode<SoundManager>("/root/SoundManager");
		optionsMenu = GetNode<Panel>("Options");
		scoreLabel = GetNode<Label>("Menu/Score");
		belts = GetNode<Node>("Scene/Belts");

		sliders = new VolumeSlider[3]{
			GetNode<VolumeSlider>("Options/MasterVolumeSlider"),
			GetNode<VolumeSlider>("Options/MasterVolumeSlider"),
			GetNode<VolumeSlider>("Options/SFXVolumeSlider")
		};
		Load();
		SetupScene();
    }

    public void QuitGame(int score)
	{
		if (bestScore < score)
		{
			bestScore = score;
			scoreLabel.Text = "Best Score : " + bestScore;
			Save();
		}
		map.QueueFree();
		Visible = true;
	}

	public void Save()
	{
		FileAccess file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Write);

		file.StoreLine(bestScore.ToString());
		foreach(VolumeSlider slider in sliders)
		{
			char muted = slider.isMuted ? '1' : '0';
			file.StoreLine(muted + " " + slider.volume.ToString());
		}
	}

	public void Load()
	{
		FileAccess file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			bestScore = 0;
			foreach(VolumeSlider slider in sliders)
				slider.Setup(0.5f, false);
			return ;
		}
		bestScore = file.GetLine().ToInt();
		foreach(VolumeSlider slider in sliders)
		{
			string[] line = file.GetLine().Split(" ");
			slider.Setup(line[1].ToFloat(), line[0] == "1");
		}
		file.Close();
		scoreLabel.Text = "Best Score : " + bestScore;
	}

	Node belts;
	Belt belt = null;
	public void SetupScene()
	{
		int type = new RandomNumberGenerator().RandiRange(2, 6);
		type = type == 2 ? 1 : type;

		Node buildings = GetNode<Node>("Scene/Buildings");
		Splitter splitter = new Splitter(new Vector2I(2, 1), LinkBelt);
		buildings.AddChild(splitter);
		belt = new Belt(new Vector2I(1, 2), null, splitter.input[0], null);
		belts.AddChild(belt);
		Operator op = new Operator(new Vector2I(0, 2), LinkBelt);
		buildings.AddChild(op);
		belt = new Belt(new Vector2I(0, 1), null, op.output[0], null);
		belts.AddChild(belt);
		Source source = new Source(new Vector2I(0, 0), LinkBelt, (ItemType)type, GetNode<Node>("Scene/Items"));
		buildings.AddChild(source);

		belt = null;
		int dif = type == 6 ? 0 : 1;
		dif = type == 1 ? 2 : dif;
		Shop shop = new Shop(new Vector2I(4, -2), LinkBelt, (ItemType)(type+dif));
		shop.SetDemo();
		buildings.AddChild(shop);
		belt = new Belt(new Vector2I(3, -2), null, shop.input[0], null);
		belts.AddChild(belt);
		belt = new Belt(new Vector2I(3, -1), null, belt, null);
		belts.AddChild(belt);
		belt = new Belt(new Vector2I(3, 0), null, belt, null);
		belts.AddChild(belt);
		belt.Connect(splitter.output[0]);
		
		belt = null;
		dif = type < 1 ? 0 : 1;
		dif = type == 3 ? 2 : dif;
		Shop shop1 = new Shop(new Vector2I(5, 2), LinkBelt, (ItemType)(type-dif));
		shop1.SetDemo();
		buildings.AddChild(shop1);
		belt = new Belt(new Vector2I(4, 2), null, shop1.input[0], null);
		belts.AddChild(belt);
		belt.Connect(splitter.output[1]);
	}

	public void LinkBelt(Belt newBelt)
	{
		if (belt != null)
		{
			belt.Connect(newBelt);
		}
		belts.AddChild(newBelt);
	}

	public void _on_play_pressed()
	{
		soundManager.PlaySFX("Start");
		map = GD.Load<PackedScene>("res://Game/Map.tscn").Instantiate<Map>();
		GetTree().Root.AddChild(map);
		map.ui.QuitToMenu += QuitGame;
		map.ui.bestScore = bestScore;
		Visible = false;
	}
	public void _on_options_pressed()
	{
		soundManager.PlaySFX("Button");
		optionsMenu.Visible = true;
	}
	public void _on_back_button_pressed()
	{
		soundManager.PlaySFX("Button");
		optionsMenu.Visible = false;
		Save();
	}
	public void _on_quit_pressed()
	{
		soundManager.PlaySFX("Button");
		GetTree().Quit();
	}
}
