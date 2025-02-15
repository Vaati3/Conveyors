using Godot;
using System;

public partial class VolumeSlider : Control
{
	[Export]string label;
	[Export]string busName;

	SoundManager soundManager;
	int busIndex;

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
		CheckBox checkBox = GetNode<CheckBox>("Mute");
		checkBox.ButtonPressed = isMuted;
		checkBox.Toggled += _on_mute_toggled;
	}

	public void _on_slider_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(value));
		// gameManager.settings.volumes[busName] = value;
	}

	public void _on_mute_toggled(bool state)
	{
		soundManager.PlaySFX("Tic");
		AudioServer.SetBusMute(busIndex, state);
		// gameManager.settings.muted[busName] = state;
	}
}
