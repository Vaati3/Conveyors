using Godot;
using System;
using System.Collections.Generic;

public enum PlaceMode {
	Remove,
	Belt,
	Splitter,
	Operator
}

public partial class GameUi : CanvasLayer
{
	public SoundManager soundManager {get; private set;}
	public PlaceMode mode{get; private set;} = PlaceMode.Belt;

	SelectionButton[] selectionButtons;


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

	int score = 0;
	Label scoreLabel;

	public override void _Ready()
	{
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		confirmPanel = GetNode<Control>("Confirm");
		gameLostPanel = GetNode<Control>("GameLost");
		pauseButton = GetNode<Button>("PauseButton");
		rewardButtonLeft = GetNode<RewardButton>("Rewards/Choices/RewardButtonLeft");
		rewardButtonRight = GetNode<RewardButton>("Rewards/Choices/RewardButtonRight");
		scoreLabel = GetNode<Label>("Score");

		pauseIcon = GD.Load<Texture2D>("res://Menus/Textures/Play.png");

		selectionButtons = new SelectionButton[4]
        {
			GetNode<SelectionButton>("Buttons/RemoveButton"),
			GetNode<SelectionButton>("Buttons/BeltButton"),
			GetNode<SelectionButton>("Buttons/SplitterButton"),
            GetNode<SelectionButton>("Buttons/OperatorButton")
        };

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

		selectionButtons[(int)mode].UpdateCount(value);
	}

	public int GetCount(PlaceMode mode)
	{
		return selectionButtons[(int)mode].count;
	}

	public void GameLost()
	{
		if (gameLostPanel.Visible == false)
		{
			GetNode<Label>("GameLost/Panel/Label").Text = "Game Over\n\nScore : " + score;
			soundManager.PlaySFX("End");
			gameLostPanel.Visible = true;
			TogglePause();
		}
	}

	public void GiveRewards()
	{
		soundManager.PlaySFX("Reward");
		TogglePause();
		RandomNumberGenerator rng = new RandomNumberGenerator();
		int a = rng.RandiRange(0, 2);
		int b = rng.RandiRange(0, 1);
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

	public void UpdateScore(int value)
	{
		score += value;
		scoreLabel.Text = "Score : " + score;
	}

	private void SelectMode(PlaceMode mode)
	{
		soundManager.PlaySFX("Tic");
		selectionButtons[(int)this.mode].Unselect();
		this.mode = mode;
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
