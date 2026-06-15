using Godot;
using System;

public partial class ActionGameUIManager : Control
{
    [Export] private Label testLabel;
    
    public override void _Ready()
    {
        var rpcManager = RpcManager.GetInstance();
        rpcManager.TestNumberChanged += OnTestNumberChanged;
    }

    private void OnTestNumberChanged(int newValue)
    {
        testLabel.Text = $"Current count: {newValue}";
    }
}
