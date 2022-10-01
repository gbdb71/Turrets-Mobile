public class LostPanel : BasePanel
{
    private void Awake()
    {
        Headquarters.OnDeath += Show;
    }

    private void OnDestroy()
    {
        Headquarters.OnDeath -= Show;
    }

    protected override void Show()
    {
        _content.gameObject.SetActive(true);
    }
}
