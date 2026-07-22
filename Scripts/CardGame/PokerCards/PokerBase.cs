using Godot;
using System;

public partial class PokerBase : Node2D
{
	private CardGameBase cardGameBase;
	public PokerGameManager PokerGameManager { get; private set; }
	[Export] public PokerContent PokerContent { get; private set; }
	[Export] public PokerDragging PokerDraggingRef{ get; private set; }
	[Export] public PokerModifiersManager PokerModifiersManager { get; private set; }

	public Action PokerSummoned;
	public bool IsLocked { get; private set; }
	public bool IsSummoned { get; private set; }
	
	public PokerState PokerState => PokerModifiersManager.PokerState;
	public PokerType PokerType => PokerModifiersManager.PokerType;


	public override void _Ready()
	{
		cardGameBase = GetTree().GetCurrentScene() as CardGameBase;
		if (cardGameBase == null)
		{
			GD.PrintErr("CardGameBase not found");
			return;
		}
		PokerGameManager = cardGameBase.PokerGameManager;
		PokerSummoned += OnPokerSummoned;
	}

	public void InitPoker(PokerInfo pokerInfo)
	{
		PokerContent.ChangePokerInfo(pokerInfo);
	}
	
	public void SetPokerLock(bool isLocked)
	{
		IsLocked = isLocked;
	}
	
	private void OnPokerSummoned()
	{
		IsSummoned = true;
	}
}
