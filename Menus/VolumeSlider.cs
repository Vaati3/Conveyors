using Godot;
using System;

public partial class VolumeSlider : Control
{
	[Export]string label;
	[Export]string busName;

	SoundManager soundManager;
	int busIndex;

	public bool isMuted {get; private set;}
	public float volume {get; private set;}

	public override void _Ready()
	{
		soundManager = GetNode<SoundManager>("/root/SoundManager");

		GetNode<Label>("Label").Text = label;
		busIndex = AudioServer.GetBusIndex(busName);
	}

	public void Setup(float volume, bool isMuted)
	{
		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(volume));
		GetNode<HSlider>("Slider").Value = volume;
		AudioServer.SetBusMute(busIndex, isMuted);
		GetNode<CheckBox>("Mute").ButtonPressed = isMuted;
	}

	public void _on_slider_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(value));
		volume = value;
	}

	public void _on_mute_toggled(bool state)
	{
		soundManager.PlaySFX("Tic");
		AudioServer.SetBusMute(busIndex, state);
		isMuted = state;
	}
}
