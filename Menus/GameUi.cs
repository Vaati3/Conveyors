using Godot;
using System;
using System.Collections.Generic;

public enum PlaceMode {
	Remove,
	Belt,
	Operator,
	Splitter,
	Merger,
	Teleporter
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
	public delegate void QuitToMenuEventHandler(int score);
	public QuitToMenuEventHandler QuitToMenu;

	Timer rewardTimer;
	RewardButton rewardButtonLeft;
	RewardButton rewardButtonRight;

	int score = 0;
	public int highScore;
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

		int len =  Enum.GetNames(typeof(PlaceMode)).Length;
		selectionButtons = new SelectionButton[len];
		for (int i = 0; i < len; i++)
			selectionButtons[i] = GetNode<SelectionButton>("Buttons/" + ((PlaceMode)i).ToString());

		rewardPanel = GetNode<Control>("Rewards");
		rewardButtonLeft.RewardSelected += RewardSelected;
		rewardButtonRight.RewardSelected += RewardSelected;

		rewardTimer = new Timer() {
			Autostart = true,
			OneShot = false,
			WaitTime = 100
		};
		AddChild(rewardTimer);
		rewardTimer.Timeout += GiveRewards;

		Pause += PauseFiller;
		
	}

	public void ChangeCount(PlaceMode mode, int value)
	{
		if (mode == PlaceMode.Remove)
			return;

		selectionButtons[(int)mode].UpdateCount(value);
	}

	public void UpdateMode()
	{
		if (mode != PlaceMode.Belt && selectionButtons[(int)mode].count < 1)
			selectionButtons[(int)PlaceMode.Belt].Pressed();
	}

	public int GetCount(PlaceMode mode)
	{
		return selectionButtons[(int)mode].count;
	}

	public void GameLost()
	{
		if (gameLostPanel.Visible == false)
		{
			highScore = score > highScore ? score : highScore;
			GetNode<Label>("GameLost/Panel/Label").Text = "Game Over\n\nHighScore : " + highScore + "\nScore : " + score;
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
		int a = rng.RandiRange(1, 5);
		int b = rng.RandiRange(1, 4);
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

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("Pause"))
		{
			TogglePause();
		}
    }

    public void _on_menu_button_pressed()
	{
		soundManager.PlaySFX("Button");
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
		soundManager.PlaySFX("Button");
		QuitToMenu(score);
	}

	public void _on_no_pressed()
	{
		soundManager.PlaySFX("Button");
		confirmPanel.Visible = false;
		TogglePause();
	}

	private void PauseFiller(bool isPaused) {}
}
