using Godot;
using System;
using Godot.Collections;

public partial class OrbitAttack : Node2D
{
    public Array<BulletBase> Bullets {get; private set;}
    [Export] public float RotationSpeed = 200f , OrbitRadius = 80f;

    private float spreadAngle;

    public override void _Ready()
    {
        Bullets = new Array<BulletBase>();
        CheckAreSameBullets();
        spreadAngle = CalculateSpreadAngle();
        PrepareBullets();
    }

    public override void _PhysicsProcess(double delta)
    {
        OrbitProcess(delta);
    }

    private float CalculateSpreadAngle()
    {
        return (Mathf.Tau / Bullets.Count); // Tau = built-in circle constant (radian)
    }

    private void PrepareBullets()
    {
        for (var i = 0; i < Bullets.Count; i++)
        {
            var currentAngle = spreadAngle * i; // how much to rotate
            GD.Print(spreadAngle);
            Vector2 offset = Vector2.FromAngle(currentAngle) * OrbitRadius; // calculate rotated unit vector(build-in) then multiply by radius
            Bullets[i].Position = Position + offset;
            Bullets[i].SetBulletSpeed(0f); // orbiting bullets does not require linear speed
        }
    }

    private void OrbitProcess(double delta)
    {
        RotationDegrees += RotationSpeed * (float)delta;
    }
    public bool CheckAreSameBullets() // reusable method for bullet list checking
    {
        var children = GetChildren(false);
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
            throw new Exception("Bullet array is empty");
        }
        var tempPointer = Bullets[0];
        foreach (var bullet in Bullets)
        {
            if (tempPointer.GetType() != bullet.GetType())
            {
                GD.PrintErr("Bullet doesn't match, is this intentional?");
                return false;
            }
        }
        return true;
    }
    
    
}
