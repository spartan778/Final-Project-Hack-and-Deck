using Godot;
using System;
using Godot.Collections;
using Godot.NativeInterop;

public partial class BulletManager : Node
{
    [Export] private Area2D bulletCleanerArea;
    public Action<BulletBase> BulletSpawned;
    public Array<BulletBase> BulletList;

    public override void _Ready()
    {
        
    }

    private void ConnectSignals()
    {
        bulletCleanerArea.AreaEntered += OnAreaOutOfBound;
        BulletSpawned += OnBulletSpawned;
    }

    private void OnAreaOutOfBound(Area2D area)
    {
        if (area is BulletBase bullet)
        {
            GD.Print("BULLET BASE AREA EXITED");
            area.QueueFree();
        }
        area?.QueueFree();
    }

    public void OnBulletSpawned(BulletBase bullet) // track all bullets in manager
    {
        BulletList.Add(bullet);
        AddChild(bullet);
        GD.Print($"Bullet Spawned: {bullet.GetType().Name}");
    }
}
