using UnityEngine;
using Cinemachine;
using Zenject;
using DG.Tweening;
using TMPro;

public class AmmoBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private float _fadeDuration = 0.5f;

    [Inject] private CinemachineVirtualCamera _playerCamera;

    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }

    public void EnableBar()
    {
        content.gameObject.SetActive(true);
        _canvasGroup.DOFade(1f, _fadeDuration);
    }

    private void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);
    }

    public void ChangeValue(int currentValue, int maxValue)
    {
        titleText.text =  $"{currentValue}/{maxValue}";
    }

    public void DisableBar()
    {
        _canvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            content.gameObject.SetActive(false);
        });
    }
}
