using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Cinemachine;
using DG.Tweening;

public class HPBar : MonoBehaviour
{
    [Inject]
    private CinemachineVirtualCamera _playerCamera;

    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI _valueText;

    [SerializeField] private Transform content;

    [SerializeField] private float _startValue;
    [SerializeField] private float _hpCoefficient;

    [SerializeField] private float moveHeight = 30f;
    [SerializeField] private float moveDuration = 0.5f;

    [SerializeField] private float fillTime = 0.5f;

    public void InitializationBar(float value)
    {
        _startValue = value;
        _hpCoefficient = 1 / _startValue;
        fillImage.fillAmount = 1f;

        _valueText.text = _startValue.ToString();
        DisableBar();
    }
    
    [ContextMenu("Enable")]
    public void EnableBar()
    {
        content.DOLocalMoveY(moveHeight, moveDuration).SetLoops(-1, LoopType.Yoyo);
        content.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);
    }

    public void ChangeValue(float newValue)
    {
        if (!content.gameObject.active)
            EnableBar();

        _valueText.text = ((int)newValue).ToString();

        newValue *= _hpCoefficient;
        DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, newValue, fillTime);
    }

    public void DisableBar()
    {
        content.gameObject.SetActive(false);
    }
}
