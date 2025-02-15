using Godot;
using System;

public partial class MainMenu : Control
{
	Map map;
	SoundManager soundManager;

	Panel optionsMenu;

    public override void _Ready()
    {
        soundManager = GetNode<SoundManager>("/root/SoundManager");
		optionsMenu = GetNode<Panel>("Options");
    }

    public void QuitGame()
	{
		map.QueueFree();
		Visible = true;
	}

	public void _on_play_pressed()
	{
		soundManager.PlaySFX("Start");
		map = GD.Load<PackedScene>("res://Game/Map.tscn").Instantiate<Map>();
		GetTree().Root.AddChild(map);
		map.ui.QuitToMenu += QuitGame;
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
	}
	public void _on_quit_pressed()
	{
		soundManager.PlaySFX("Button");
		GetTree().Quit();
	}
}
