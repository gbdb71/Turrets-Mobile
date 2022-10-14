using Zenject;

public class LoadingScene : SceneLoader
{
    [Inject] private LevelManager _levelManager;

    private void Start()
    {
        _targetScene = _levelManager.CurrentLevel;
        StartLoadScene();
    }

    protected override void ShowProgress(string progress)
    {
    }
}
