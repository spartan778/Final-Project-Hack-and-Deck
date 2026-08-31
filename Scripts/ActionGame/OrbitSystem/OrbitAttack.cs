using Godot;
using System;
using Godot.Collections;

public partial class OrbitAttack : Node2D
{
    public Array<BulletBase> Bullets { get; private set; }
    [Export] public float RotationSpeed = 200f, OrbitRadius = 80f;
    [Export] private PackedScene bulletPrefab;
    [Export] public int DefaultBulletCount = 3;
    [Export] private Timer orbitResetTimer;
    [Export] public float OrbitResetTime = 5f;

    public int CurrentBulletCount, BonusBulletCount = 0;
    private float spreadAngle;

    public override void _Ready()
    {
        Bullets = new Array<BulletBase>();
        CurrentBulletCount = DefaultBulletCount;
        
        if (orbitResetTimer != null) // condition designed to allow non-automatic refresh
        {
            orbitResetTimer.WaitTime = OrbitResetTime;
            orbitResetTimer.Timeout += OnResetTimerTimeout; 
            orbitResetTimer.Start();
            UpdateOrbit();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        OrbitProcess(delta);
    }

    public void UpdateOrbit() // core method
    {
        ResetOrbit();
        InitBullets();
        CheckAndLoadBullets();
        spreadAngle = CalculateSpreadAngle();
        SpreadBullets();
    }

    private void OnResetTimerTimeout()
    {
        UpdateOrbit();
    }
    private void InitBullets()
    {
        CurrentBulletCount = DefaultBulletCount + BonusBulletCount;
        var bullet = bulletPrefab.InstantiateOrNull<BulletBase>();
        if (bullet == null)
        {
            GD.PrintErr("Bullet prefab doesn't exist/wrong prefab type");
            return;
        }
        AddChild(bullet);
        if (DefaultBulletCount > 1)
        {
            for (var i = 0; i < CurrentBulletCount-1; i++)
            {
                var temp = bulletPrefab.Instantiate<BulletBase>();
                AddChild(temp);
            }
        }
    }
    private float CalculateSpreadAngle()
    {
        return (Mathf.Tau / CurrentBulletCount); // Tau = built-in circle constant (radian)
    }

    private void SpreadBullets()
    {
        for (var i = 0; i < Bullets.Count; i++)
        {
            var currentAngle = spreadAngle * i; // how much to rotate
            // GD.Print(spreadAngle);
            Vector2 offset = Vector2.FromAngle(currentAngle) * OrbitRadius; // calculate rotated unit vector(build-in) then multiply by radius
            Bullets[i].Position = Position + offset;
            Bullets[i].SetBulletSpeed(0f); // orbiting bullets does not require linear speed
        }
        // GD.Print($"Orbit Count: {Bullets.Count}");
        // GD.Print($"Current Bullet Count: {CurrentBulletCount}");
    }

    private void OrbitProcess(double delta)
    {
        RotationDegrees += RotationSpeed * (float)delta;
    }
    public bool CheckAndLoadBullets() // reusable method for bullet list checking
    {
        var children = GetChildren();
        Bullets.Clear();
        foreach (var node in children)
        {
            if (node is BulletBase bullet)
            {
                Bullets.Add(bullet);
            }
        }
        if (Bullets.Count <= 0)
        {
            GD.PrintErr("There are no bullets");
            return false;
        }
        var tempPointer = Bullets[0];
        foreach (var bullet in Bullets)
        {
            if (tempPointer.GetType() != bullet.GetType())
            {
                GD.PrintErr("Bullet doesn't match, is this intentional?");
                // return true;
            }
        }
        return true;
    }
    
    public void SetBonusBulletCount(int bonusCount)
    {
        BonusBulletCount = bonusCount;
    }

    public void ResetOrbit()
    {
        var children = GetChildren();
        Bullets.Clear();
        CurrentBulletCount = DefaultBulletCount;
        foreach (var node in children)
        {
            if (node is BulletBase bullet)
            {
                bullet.QueueFree();
            }
        }
    }
    
}
