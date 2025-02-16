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


		sliders = new VolumeSlider[3]{
			GetNode<VolumeSlider>("Options/MasterVolumeSlider"),
			GetNode<VolumeSlider>("Options/MasterVolumeSlider"),
			GetNode<VolumeSlider>("Options/SFXVolumeSlider")
		};
		Load();
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
