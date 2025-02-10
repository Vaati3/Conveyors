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

	public bool isPaused{get; private set;} = false;
	
	Button pauseButton;
	Texture2D pauseIcon;
	Panel confirmPanel;

	Label beltCountLabel;
	public int beltCount {get; private set;} = 20;

	public override void _Ready()
	{
		confirmPanel = GetNode<Panel>("Confirm");
		pauseButton = GetNode<Button>("PauseButton");

		pauseIcon = GD.Load<Texture2D>("res://Menus/Textures/Play.png");

		beltCountLabel = GetNode<Label>("Buttons/BeltButton/Count");
		beltCountLabel.Text = beltCount.ToString();
	}

	public void ChangeBeltCount(int value)
	{
		beltCount += value;

		beltCountLabel.Text = beltCount.ToString();
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

	public void _on_pause_button_pressed()
	{
		isPaused = !isPaused;
		Texture2D buffer = pauseButton.Icon;
		pauseButton.Icon = pauseIcon;
		pauseIcon = buffer;

		Pause(isPaused);
	}

	public delegate void PauseEventHandler(bool state);
	public PauseEventHandler Pause;

	public void _on_yes_pressed()
	{
		QuitToMenu();
	}

	public void _on_no_pressed()
	{
		confirmPanel.Visible = false;
	}
}
