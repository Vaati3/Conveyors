using Godot;
using System;

public partial class MainMenu : Control
{
	Map map;
	public void QuitGame()
	{
		map.QueueFree();
		Visible = true;
	}

	public void _on_play_pressed()
	{
		map = GD.Load<PackedScene>("res://Game/Map.tscn").Instantiate<Map>();
		GetTree().Root.AddChild(map);
		map.ui.QuitToMenu += QuitGame;
		Visible = false;
	}
	public void _on_options_pressed()
	{
		GD.Print("Options");
	}
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
