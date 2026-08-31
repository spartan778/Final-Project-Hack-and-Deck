using Godot;
using System;

public partial class QiRegenVfx : AnimatedSprite2D
{
    [Export] private QiSystem qiSystemRef;
    public override void _Ready()
    {
        qiSystemRef.QiRegenAction += OnQiRegen;
        AnimationFinished += OnAnimationFinished;
        Visible = false;
    }

    private void OnQiRegen()
    {
        Visible = true;
        Play("QiRegen");
    }

    private void OnAnimationFinished()
    {
        Visible = false;
    }
}
