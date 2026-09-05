using Godot;
using System;

public partial class MagicBulletAttack : Node
{
    [Export] private PackedScene magicBulletPrefab;
    [Export] private PlayerManager playerManagerRef;
    [Export] private AudioStreamPlayer magicBulletSound;
    [Export] public float AttackArc;
    
    private BulletManager bulletManagerRef;


    public override void _Ready()
    {
        bulletManagerRef = ActionGameBase.Instance.BulletManagerRef;
    }
    public void MakeMagicAttack(int bulletAmount)
    {
        var separation = AttackArc / bulletAmount; // spread bullets depending on amount
        var startAngle = -(AttackArc/2); //line up the middle bullet at center
        
        for (var i = 0; i < bulletAmount; i++)
        {
            var bullet = magicBulletPrefab.Instantiate<MagicBullet>();
            bullet.GlobalPosition = playerManagerRef.GetGlobalPosition();
            var rotateInRad = Mathf.DegToRad(startAngle + separation * i);
            // GD.Print($"Bullet Angle: {Mathf.RadToDeg(rotateInRad)}");
            var finalVector = playerManagerRef.GetMouseToPlayerVector().Rotated(rotateInRad);
            bullet.InitBullet(finalVector);
            // AddChild(bullet);
            bulletManagerRef.BulletSpawned?.Invoke(bullet);
        }
        GD.Print($"MagicAttack with {bulletAmount} bullet");
    }

    public void MakeMagicAttack(int bulletAmount, Vector2 finalVector)
    {
        var separation = AttackArc / bulletAmount; // spread bullets depending on amount
        var startAngle = -(AttackArc/2); //line up the middle bullet at center
        
        for (var i = 0; i < bulletAmount; i++)
        {
            var bullet = magicBulletPrefab.Instantiate<MagicBullet>();
            bullet.GlobalPosition = playerManagerRef.GetGlobalPosition();
            var rotateInRad = Mathf.DegToRad(startAngle + separation * i);
            // GD.Print($"Bullet Angle: {Mathf.RadToDeg(rotateInRad)}");
            finalVector = finalVector.Normalized();
            bullet.InitBullet(finalVector);
            // AddChild(bullet);
            bulletManagerRef.BulletSpawned?.Invoke(bullet);
        }
        magicBulletSound.Play();
        GD.Print($"MagicAttack (Directional) with {bulletAmount} bullet");
    }
}
