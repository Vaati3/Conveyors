using Godot;
using System;
using System.Collections.Generic;

public enum PlaceMode {
	Remove = -1,
	Belt,
	Splitter,
	Operator
}

public partial class GameUi : CanvasLayer
{
	public SoundManager soundManager {get; private set;}
	public PlaceMode mode{get; private set;} = PlaceMode.Belt;

	Label[] countLabels;
	public int[] counts {get; private set;}

	Control confirmPanel;
	Control gameLostPanel;
	Control rewardPanel;

	Texture2D pauseIcon;
	Button pauseButton;
	public bool isPaused{get; private set;} = false;

	public delegate void PauseEventHandler(bool state);
	public PauseEventHandler Pause;
	public delegate void QuitToMenuEventHandler();
	public QuitToMenuEventHandler QuitToMenu;


	Timer rewardTimer;
	RewardButton rewardButtonLeft;
	RewardButton rewardButtonRight;

	public override void _Ready()
	{
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		confirmPanel = GetNode<Control>("Confirm");
		gameLostPanel = GetNode<Control>("GameLost");
		pauseButton = GetNode<Button>("PauseButton");
		rewardButtonLeft = GetNode<RewardButton>("Rewards/Choices/RewardButtonLeft");
		rewardButtonRight = GetNode<RewardButton>("Rewards/Choices/RewardButtonRight");

		pauseIcon = GD.Load<Texture2D>("res://Menus/Textures/Play.png");

		countLabels = new Label[3]
        {
            GetNode<Label>("Buttons/BeltButton/Count"),
			GetNode<Label>("Buttons/SplitterButton/Count"),
			GetNode<Label>("Buttons/OperatorButton/Count")
        };
		counts = new int[3] {
			20,
			1,
			5
		};
		
		for (int i = 0; i < counts.Length; i++)
			countLabels[i].Text = counts[i].ToString();

		rewardPanel = GetNode<Control>("Rewards");
		rewardButtonLeft.RewardSelected += RewardSelected;
		rewardButtonRight.RewardSelected += RewardSelected;

		rewardTimer = new Timer() {
			Autostart = true,
			OneShot = false,
			WaitTime = 110
		};
		AddChild(rewardTimer);
		rewardTimer.Timeout += GiveRewards;
	}

	public void ChangeCount(PlaceMode mode, int value)
	{
		if (mode == PlaceMode.Remove)
			return;
		int index = (int)mode;
		counts[index] += value;

		countLabels[index].Text = counts[index].ToString();
	}

	public int GetCount(PlaceMode mode)
	{
		return counts[(int)mode];
	}

	public void GameLost()
	{
		soundManager.PlaySFX("End");
		gameLostPanel.Visible = true;
		TogglePause();
	}

	public void GiveRewards()
	{
		soundManager.PlaySFX("Reward");
		TogglePause();
		RandomNumberGenerator rng = new RandomNumberGenerator();
		int a = rng.RandiRange(0, 2);
		int b = rng.RandiRange(0, 2);
		b += b >= a ? 1 : 0;
		rewardButtonLeft.Update((PlaceMode)a);
		rewardButtonRight.Update((PlaceMode)b);
		rewardPanel.Visible = true;
	}

	public void RewardSelected(int beltAmount, PlaceMode building, int buildingAmount)
	{
		ChangeCount(PlaceMode.Belt, beltAmount);
		if (building > PlaceMode.Belt)
			ChangeCount(building, buildingAmount);
		rewardPanel.Visible = false;
		TogglePause();
	}

	public void _on_remove_button_pressed()
	{
		soundManager.PlaySFX("Tic");
		mode = PlaceMode.Remove;
	}
	public void _on_belt_button_pressed()
	{
		soundManager.PlaySFX("Tic");
		mode = PlaceMode.Belt;
	}
	public void _on_splitter_button_pressed()
	{
		soundManager.PlaySFX("Tic");
		mode = PlaceMode.Splitter;
	}
	public void _on_opertator_button_pressed()
	{
		soundManager.PlaySFX("Tic");
		mode = PlaceMode.Operator;
	}

	public void _on_menu_button_pressed()
	{
		soundManager.PlaySFX("button");
		confirmPanel.Visible = true;
		TogglePause();
	}

	public void TogglePause()
	{
		isPaused = !isPaused;
		Texture2D buffer = pauseButton.Icon;
		pauseButton.Icon = pauseIcon;
		pauseIcon = buffer;
		rewardTimer.Paused = isPaused;

		Pause(isPaused);
	}

	public void _on_yes_pressed()
	{
		soundManager.PlaySFX("button");
		QuitToMenu();
	}

	public void _on_no_pressed()
	{
		soundManager.PlaySFX("button");
		confirmPanel.Visible = false;
		TogglePause();
	}
}
