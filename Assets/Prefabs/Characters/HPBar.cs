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
    private float _value;

    [Inject]
    private CinemachineVirtualCamera _playerCamera;

    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI _valueText;

    [SerializeField] private float moveHeight = 30f;
    [SerializeField] private float moveDuration = 0.5f;

    public void InitializationBar(float value)
    {
        _value = value;
        _valueText.text = _value.ToString();
    }
    
    [ContextMenu("Enable")]
    public void EnableBar()
    {
        transform.DOLocalMoveY(moveHeight, moveDuration).SetLoops(-1, LoopType.Yoyo);
    }

    public void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);
    }

    public void ChangeValue()
    {

    }

    public void DisableBar()
    {
        
    }
}
