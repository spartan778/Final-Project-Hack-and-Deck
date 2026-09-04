using Godot;
using System;

public partial class ActionGameSoundManager : Node
{
    [Export] private AudioStreamPlayer BGMPlayer;

    public override void _Ready()
    {
        BGMPlayer.Play();
    }
}
