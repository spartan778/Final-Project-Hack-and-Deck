using Godot;
using System;

public partial class QiBlastAttack : Node2D
{
    [Export] private PackedScene qiBlastPrefab;
    [Export] private PlayerManager playerManagerRef;
    private BulletManager bulletManagerRef;
    
    public override void _Ready()
    {
        bulletManagerRef = ActionGameBase.Instance.BulletManagerRef;
    }
    
    public void MakeQiBlast()
    {
        var projectile = qiBlastPrefab.Instantiate<QiBlast>();
        projectile.GlobalPosition = playerManagerRef.GetGlobalPosition();
        var finalVector = playerManagerRef.GetMouseToPlayerVector();
        projectile.InitBullet(finalVector);
        bulletManagerRef.BulletSpawned?.Invoke(projectile);
    }
}
