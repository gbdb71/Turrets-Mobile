using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Zenject;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class AmmoBar : MonoBehaviour
{
    [Inject]
    private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }

    [ContextMenu("Enable")]
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
        titleText.text = currentValue + "/" + maxValue;
    }

    [ContextMenu("Disable")]
    public void DisableBar()
    {
        _canvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            content.gameObject.SetActive(false);
        });
    }
}
