using Godot;
using System;

public partial class MainMenu : Control
{
	Map map;
	SoundManager soundManager;

    public override void _Ready()
    {
        soundManager = GetNode<SoundManager>("/root/SoundManager");
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
		GD.Print("Options");
	}
	public void _on_quit_pressed()
	{
		soundManager.PlaySFX("Button");
		GetTree().Quit();
	}
}
