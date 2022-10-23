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

    [SerializeField] private Slider fillImage;
    [SerializeField] private TextMeshProUGUI _valueText;

    [SerializeField] private Transform content;

    [SerializeField] private float _startValue;
    [SerializeField] private float _hpCoefficient;

    [SerializeField] private float moveHeight = 30f;
    [SerializeField] private float moveDuration = 0.5f;

    [SerializeField] private float fillTime = 0.5f;

    [SerializeField] private float rotateTime = 0.05f;
    [SerializeField] private float rotateStrength = 3f;


    public void InitializationBar(float value)
    {
        _startValue = value;
        _hpCoefficient = 1 / _startValue;
        fillImage.value = 1f;

        _valueText.text = _startValue.ToString();
        DisableBar();
    }

    [ContextMenu("Enable")]
    public void EnableBar()
    {
        // content.DOLocalMoveY(moveHeight, moveDuration).SetLoops(-1, LoopType.Yoyo);
        content.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);
    }

    public void ChangeValue(float newValue)
    {
        if (!content.gameObject.activeSelf)
            EnableBar();

        _valueText.text = ((int)newValue).ToString();

        newValue *= _hpCoefficient;
        fillImage.DOValue(newValue, fillTime).SetEase(Ease.OutBack);

        var s = DOTween.Sequence();
        s.Append(content.transform.DOLocalRotate(new Vector3(0, 0, rotateStrength), rotateTime));
        s.Append(content.transform.DOLocalRotate(new Vector3(0, 0, -rotateStrength), rotateTime));
        s.Append(content.transform.DOLocalRotate(new Vector3(0, 0, 0), rotateTime));

    }

    public void DisableBar()
    {
        content.gameObject.SetActive(false);
    }
}
