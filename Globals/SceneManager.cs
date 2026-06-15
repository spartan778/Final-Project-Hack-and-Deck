using Godot;
using System;

public partial class SceneManager : Node
{
    private static SceneManager instance;
    [Export] private PackedScene actionGameScene, cardGameScene;
    private PackedScene preparedScene;


    public override void _EnterTree()
    {
        instance = this;
    }
    public static SceneManager GetInstance()
    {
        if (instance == null)
        {
            GD.PrintErr("No SceneManager instance found");
        }
        return instance;
    }

    public void PrepareMainGameScene()
    {
        preparedScene = NetworkManager_Singleton.GetInstance().IsHost ? actionGameScene : cardGameScene;
    }
    public void ChangeToMainGameScene()
    {
        GetTree().ChangeSceneToPacked(preparedScene);
    }
}
