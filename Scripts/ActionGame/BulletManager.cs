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
        BulletList = new Array<BulletBase>(); //init bullet list
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        bulletCleanerArea.AreaEntered += OnAreaOutOfBound;
        BulletSpawned += OnBulletSpawned;
    }

    private void OnAreaOutOfBound(Area2D area)
    {
        // GD.Print("area out of bound");
        if (area is BulletBase bullet)
        {
            area.QueueFree();
            // GD.Print($"{bullet.Name} is out of range");
        }
        // area?.QueueFree();
    }

    public void OnBulletSpawned(BulletBase bullet) // track all bullets in manager
    {
        BulletList.Add(bullet);
        AddChild(bullet);
        
        // GD.Print($"Bullet Spawned: {bullet.GetType().Name}");
    }
}
