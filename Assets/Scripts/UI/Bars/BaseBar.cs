using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class BaseBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _reloadIndicator;
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

    public void EnableReloadIndicator(float time)
    {
        if (_reloadIndicator != null)
        {
            _reloadIndicator.gameObject.SetActive(true);

            _reloadIndicator.transform.DOLocalRotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360).OnComplete(() =>
            {
                _reloadIndicator.gameObject.SetActive(false);
            });
        }
    }

    public abstract void ChangeValue(int currentValue, int maxValue);
}
