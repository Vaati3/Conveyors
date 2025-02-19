using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SoundManager : Node
{
    const int playerAmount = 8;
    AudioStreamPlayer[] players;
    AudioStreamPlayer musicPlayer;
    Dictionary<string, AudioStream> sfx;
    Dictionary<string, AudioStream> musics;
    Queue<AudioStream> sfxQueue;

    private void CreatePlayers()
    {
        musicPlayer = new AudioStreamPlayer();
        musicPlayer.Bus = "Music";
        AddChild(musicPlayer);
        players = new AudioStreamPlayer[playerAmount];
        
        for (int i = 0; i < playerAmount; i++)
        {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.Bus = "SFX";
            AddChild(player);
            players[i] = player;
        }
    }

    private void  PlayStream(AudioStream stream)
    {
        for(int i = 0; i < playerAmount; i++)
        {
            if(!players[i].Playing)
            {
                players[i].Stream = stream;
                players[i].Play();
                return ;
            }
        }
        return;
    }

    private Dictionary<string, AudioStream> LoadSounds(string path)
    {
        DirAccess dir = DirAccess.Open(path);   
        if (dir == null)
            return null;

        Dictionary<string, AudioStream> sounds = new Dictionary<string, AudioStream>();
        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while(fileName != "")
        {
            if (!dir.CurrentIsDir() && fileName.EndsWith(".import"))
            {
                fileName = fileName.Split(".import")[0];
                AudioStream audio = GD.Load<AudioStream>(path + fileName);
                if (audio != null)
                    sounds.Add(fileName.GetBaseName(), audio);
            }
            fileName = dir.GetNext();
        }
        return sounds;
    }

    public override void _Ready()
    {
        CreatePlayers();
        
        sfx = LoadSounds("res://Sounds/SFX/");
        musics = LoadSounds("res://Sounds/Music/");
        sfxQueue = new Queue<AudioStream>();
    }

    public override void _Process(double delta)
    {
        if (sfxQueue.Count > 0)
        {
            PlayStream(sfxQueue.Dequeue());
        }
    }

    public void PlaySFX(string sfxName)
    {
        AudioStream audio;
        if (sfx.TryGetValue(sfxName, out audio))
        {
            sfxQueue.Enqueue(audio);
        }
    }

    public void PlayMusic(string musicName)
    {
        AudioStream audio;
        if (musics.TryGetValue(musicName, out audio))
        {
            musicPlayer.Stop();
            musicPlayer.Stream = audio;
            musicPlayer.Play();
        }
    }

    public void StopSFX(string sfxName)
    {
        foreach(AudioStreamPlayer player in players)
        {
            if (player.Playing && player.Stream.ResourcePath.GetFile().GetBaseName() == sfxName)
            {
                player.Stop();
                return;
            }
        }
    }
}