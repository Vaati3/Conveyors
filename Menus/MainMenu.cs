using Godot;
using System;

public partial class MainMenu : Control
{
	Map map;
	SoundManager soundManager;

	Panel optionsMenu;
	Panel credits;

	VolumeSlider[] sliders;

	const string saveFile = "user://save.save";
	int highScore;
	Label scoreLabel;
	CheckBox tutoCheck;

    public override void _Ready()
    {
        soundManager = GetNode<SoundManager>("/root/SoundManager");
		optionsMenu = GetNode<Panel>("Options");
		credits = GetNode<Panel>("Credits");
		scoreLabel = GetNode<Label>("Menu/Score");
		tutoCheck = GetNode<CheckBox>("Options/TutorialCheck");
		belts = GetNode<Node>("Scene/Belts");

		sliders = new VolumeSlider[3]{
			GetNode<VolumeSlider>("Options/MasterVolumeSlider"),
			GetNode<VolumeSlider>("Options/MusicVolumeSlider"),
			GetNode<VolumeSlider>("Options/SFXVolumeSlider")
		};
		Load();
		SetupScene();
		soundManager.PlayMusic("Music");
    }

    public void QuitGame(int score)
	{
		if (highScore < score)
		{
			highScore = score;
			scoreLabel.Text = "HighScore : " + highScore;
			Save();
		}
		map.QueueFree();
		Visible = true;
	}

	public void Save()
	{
		FileAccess file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Write);

		file.StoreLine(highScore.ToString());
		foreach(VolumeSlider slider in sliders)
		{
			char muted = slider.isMuted ? '1' : '0';
			file.StoreLine(muted + " " + slider.volume.ToString());
		}
		file.StoreLine(tutoCheck.ButtonPressed ? "1" : "0");
		file.Close();
	}

	public void Load()
	{
		FileAccess file = FileAccess.Open(saveFile, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			highScore = 0;
			foreach(VolumeSlider slider in sliders)
				slider.Setup(0.5f, false);
			tutoCheck.ButtonPressed = true;
			return ;
		}
		highScore = file.GetLine().ToInt();
		foreach(VolumeSlider slider in sliders)
		{
			string[] line = file.GetLine().Split(" ");
			slider.Setup(float.Parse(line[1]), line[0] == "1");
		}
		tutoCheck.ButtonPressed = file.GetLine() == "1";
		file.Close();
		scoreLabel.Text = "Best Score : " + highScore;
	}

	Node belts;
	Belt belt = null;
	public void SetupScene()
	{
		int type = new RandomNumberGenerator().RandiRange(2, 5);
		type = type == 2 ? 1 : type;

		Node buildings = GetNode<Node>("Scene/Buildings");
		Splitter splitter = new Splitter(new Vector2I(2, 1), LinkBelt);
		buildings.AddChild(splitter);
		belt = new Belt(new Vector2I(1, 2), null, splitter.input[0], null);
		belts.AddChild(belt);
		belt = new Belt(new Vector2I(0, 2), null, belt, null);
		belts.AddChild(belt);
		belt = new Belt(new Vector2I(0, 1), null, belt, null);
		belts.AddChild(belt);
		Source source = new Source(new Vector2I(0, 0), LinkBelt, (ItemType)type, GetNode<Node>("Scene/Items"));
		buildings.AddChild(source);

		belt = null;
		int dif = type == 1 ? 2 : 1;
		Shop shop = new Shop(new Vector2I(4, -2), LinkBelt, (ItemType)(type+dif));
		shop.SetDemo();
		buildings.AddChild(shop);
		belt = new Belt(new Vector2I(3, -2), null, shop.input[0], null);
		belts.AddChild(belt);
		Operator op = new Operator(new Vector2I(3, -1), LinkBelt);
		buildings.AddChild(op);
		belt = new Belt(new Vector2I(3, 0), null, op.output[0], null);
		belts.AddChild(belt);
		belt.Connect(splitter.output[0]);
		
		belt = null;
		Shop shop1 = new Shop(new Vector2I(5, 2), LinkBelt, (ItemType)type);
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
		map.SetupUI(highScore, QuitGame, tutoCheck);
		Visible = false;
	}
	public void _on_options_pressed()
	{
		soundManager.PlaySFX("Button");
		optionsMenu.Visible = true;
	}
	public void _on_credits_pressed()
	{
		soundManager.PlaySFX("Button");
		credits.Visible = true;
	}
	public void _on_back_button_pressed()
	{
		soundManager.PlaySFX("Button");
		if (optionsMenu.Visible)
		{
			optionsMenu.Visible = false;
			Save();
		} else {
			credits.Visible = false;
		}
	}
	public void _on_quit_pressed()
	{
		soundManager.PlaySFX("Button");
		GetTree().Quit();
	}
}
