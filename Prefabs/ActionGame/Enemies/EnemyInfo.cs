using Godot;
using System;

[GlobalClass]
public partial class EnemyInfo : Resource
{
    [Export] public PackedScene EnemyPrefab;
    [Export] public float SpawnValue;
}
