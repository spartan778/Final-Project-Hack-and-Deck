using Godot;
using System;
using Godot.Collections;

public partial class QiIndicatorDisplay : Node
{
    [Export] public Array<TextureProgressBar> QiProgressBars;
    [Export] private QiSystem qiSystemRef;

    public override void _Ready()
    {
        if (Mathf.FloorToInt(qiSystemRef.MaxQi / qiSystemRef.DefaultQiAttackCost) != QiProgressBars.Count)
        {
            GD.PrintErr("Qi ProgressBars amount does not match max Qi value");
        }
    }

    public override void _Process(double delta)
    {
        UpdateProgressBars();
    }

    private void UpdateProgressBars()
    {
        var currentQi = qiSystemRef.CurrentQi;
        var filledCount = Mathf.FloorToInt(currentQi / qiSystemRef.CurrentQiAttackCost);
        for (var i = 0; i < filledCount; i++)
        {
            QiProgressBars[i].Value = 100f;
        }
        if (filledCount == QiProgressBars.Count) return;
        for (var i = QiProgressBars.Count-1; i > filledCount; i--)
        {
            QiProgressBars[i].Value = 0f;
        }
        var leftOver = (currentQi % qiSystemRef.CurrentQiAttackCost);
        QiProgressBars[filledCount].Value = leftOver;
    }
}
