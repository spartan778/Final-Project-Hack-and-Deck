using Godot;
using System;

public partial class PokerBase : Node2D
{
	private CardGameBase cardGameBase;
	public PokerGameManager PokerGameManager { get; private set; }
	[Export] public PokerContent PokerContent { get; private set; }
	[Export] public PokerDragging PokerDraggingRef{ get; private set; }
	public bool IsLocked { get; private set; }


	public override void _Ready()
	{
		cardGameBase = GetTree().GetCurrentScene() as CardGameBase;
		if (cardGameBase == null)
		{
			GD.PrintErr("CardGameBase not found");
			return;
		}
		PokerGameManager = cardGameBase.PokerGameManager;
	}

	public void InitPoker(PokerInfo pokerInfo)
	{
		PokerContent.ChangePokerInfo(pokerInfo);
	}
	
	public void SetPokerLock(bool isLocked)
	{
		IsLocked = isLocked;
	}
}
