public class FinishPanel : BasePanel
{
    private void Awake()
    {
        Game.OnGameFinished += Show;
    }

    private void OnDestroy()
    {
        Game.OnGameFinished -= Show;
    }

    protected override void Show()
    {
        _content.gameObject.SetActive(true);
    }
}
