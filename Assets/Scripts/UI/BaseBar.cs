using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class BaseBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform content;

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
        content.gameObject.SetActive(true);
        _canvasGroup.DOFade(1f, _fadeDuration);
        Debug.Log("Enable + " + gameObject.name);
    }

    public void DisableBar()
    {
        _canvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
        {
            content.gameObject.SetActive(false);
        });
    }

    public virtual void Initialization(int currentValue, int maxValue) { Debug.Log("Non"); }
    public virtual void ChangeValue(int currentValue, int maxValue) { }
}
