using Godot;
using System;

public partial class SceneManager : Node
{
    public static SceneManager Instance { get; private set; }
    [Export] private PackedScene actionGameScene, cardGameScene;
    private PackedScene preparedScene;


    public override void _EnterTree()
    {
        Instance = this;
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
