using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Zenject;

public abstract class BaseBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;

    [Inject] private CinemachineVirtualCamera _playerCamera;

    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);
    }

    public void EnableBar()
    {
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.DOFade(1f, _fadeDuration);
    }

    public void DisableBar()
    {
        _canvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            _canvasGroup.gameObject.SetActive(false);
        });
    }

    public abstract void Initialization(int currentValue, int maxValue);
    public abstract void ChangeValue(int currentValue, int maxValue);
}
