using Godot;
using System;
using System.Collections.Generic;

public enum PlaceMode {
	Remove = -1,
	Belt,
	Splitter
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

	Label[] countLabels;
	public int[] counts {get; private set;}

	public override void _Ready()
	{
		confirmPanel = GetNode<Panel>("Confirm");
		pauseButton = GetNode<Button>("PauseButton");

		pauseIcon = GD.Load<Texture2D>("res://Menus/Textures/Play.png");

		countLabels = new Label[2]
        {
            GetNode<Label>("Buttons/BeltButton/Count"),
			GetNode<Label>("Buttons/SplitterButton/Count")
        };
		counts = new int[2] {
			20,
			1
		};
		
		for (int i = 0; i < counts.Length; i++)
			countLabels[i].Text = counts[i].ToString();
	}

	public void ChangeBeltCount(PlaceMode mode, int value)
	{
		if (mode == PlaceMode.Remove)
			return;
		int index = (int)mode;
		counts[index] += value;

		countLabels[index].Text = counts[index].ToString();
	}

	public void _on_remove_button_pressed()
	{
		mode = PlaceMode.Remove;
	}
	public void _on_belt_button_pressed()
	{
		mode = PlaceMode.Belt;
	}
	public void _on_splitter_button_pressed()
	{
		mode = PlaceMode.Splitter;
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
