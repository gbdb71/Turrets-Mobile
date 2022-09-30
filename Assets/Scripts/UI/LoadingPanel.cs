using UnityEngine;
using Zenject;

public class LoadingPanel : MonoBehaviour
{
    [Inject] private Map _map;
    [SerializeField] private Transform _content;

    private void Awake()
    {
        _map.OnMapGenerated += HidePanel;
    }

    private void Start()
    {
        if(_content != null)
        {
            _content.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        _map.OnMapGenerated -= HidePanel;
    }

    private void HidePanel()
    {
        if (_content != null)
            _content.gameObject.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
