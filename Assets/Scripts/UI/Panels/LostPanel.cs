using Zenject;

public class LostPanel : BasePanel
{
    [Inject] private GameLogic _game;

    private void Awake()
    {
        GameLogic.OnGameFinished += Show;
    }

    private void OnDestroy()
    {
        GameLogic.OnGameFinished -= Show;
    }

    protected override void Show()
    {
        if(!_game.IsWin)
            _content.gameObject.SetActive(true);
    }
}
