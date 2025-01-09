using Godot;
using System;

public partial class MainMenu : Control
{
	public void _on_play_pressed()
	{
		GD.Print("Play");
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
