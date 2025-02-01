using Godot;
using System;

public enum PlaceMode {
	Remove,
	Belt
}

public partial class GameUi : CanvasLayer
{
	public PlaceMode mode{get; private set;} = PlaceMode.Belt;

	public delegate void QuitToMenuEventHandler();
	public QuitToMenuEventHandler QuitToMenu;

	Panel confirmPanel;
	public override void _Ready()
	{
		confirmPanel = GetNode<Panel>("Confirm");
	}

	public void _on_belt_button_pressed()
	{
		mode = PlaceMode.Belt;
	}

	public void _on_remove_button_pressed()
	{
		mode = PlaceMode.Remove;
	}

	public void _on_menu_button_pressed()
	{
		confirmPanel.Visible = true;
	}

	public void _on_yes_pressed()
	{
		QuitToMenu();
	}

	public void _on_no_pressed()
	{
		confirmPanel.Visible = false;
	}
}
