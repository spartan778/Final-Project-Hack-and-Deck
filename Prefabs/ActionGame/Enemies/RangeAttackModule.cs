using Godot;
using System;
using System.Collections;
using HCoroutines;

interface IRangeAttacker
{
   RangeAttackModule RangeAttackModule { get; set; }
}
public partial class RangeAttackModule : Node
{
   [Export] private PackedScene projectilePrefab;
   [Export] private Node2D attackerNode;
   [Export] public int ProjectileCount;
   
   private PlayerManager playerManagerRef;
   private BulletManager bulletManagerRef;
   private Coroutine shootingCoroutine;

   public override void _Ready()
   {
      playerManagerRef = ActionGameBase.Instance.PlayerManagerRef;
      bulletManagerRef = ActionGameBase.Instance.BulletManagerRef;
      if (projectilePrefab == null)
      {
         GD.PrintErr("No projectile scene.");
         return;
      }
      var temp = projectilePrefab.InstantiateOrNull<BulletBase>();
      if (temp == null)
      {
         GD.PrintErr("Assigned scene is not a BulletBase.");
         return;
      }
      temp.QueueFree();
   }

   public void MakeRangedAttack_Player( float interval = .5f) //basic call for making ranged attack
   {
      shootingCoroutine = Co.Run(RangedAttackCoroutine(ProjectileCount, interval));
   }

   private IEnumerator RangedAttackCoroutine(int count, float interval)
   {
      for (int i = 0; i < count; i++)
      {
         ShootProjectile_Player();
         yield return Co.Wait(interval);
      }
      shootingCoroutine = null;
   }
   public void ShootProjectile_Player()
   {
      var bulletVector = playerManagerRef.Position - attackerNode.Position;
      var projectile = projectilePrefab.Instantiate<BulletBase>();
      projectile.GlobalPosition = attackerNode.GlobalPosition;
      projectile.InitBullet(bulletVector);
      bulletManagerRef.BulletSpawned?.Invoke(projectile);
      GD.Print($"Shooting from: {attackerNode.Name}");
   }

   public override void _ExitTree()
   {
      if (shootingCoroutine != null)
      {
         shootingCoroutine.Kill();
      }
   }
}
