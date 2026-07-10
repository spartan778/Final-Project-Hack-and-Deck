using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class EnemyList : Resource
{
    [Export] public Array<EnemyInfo> EnemyInfos;
    [Export] public EnemyInfo DefaultEnemy;
}
