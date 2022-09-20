using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Zenject;
using TMPro;

public class AmmoBar : MonoBehaviour
{
    [Inject]
    private CinemachineVirtualCamera _playerCamera;

    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI titleText;

    [ContextMenu("Enable")]
    public void EnableBar()
    {
        content.gameObject.SetActive(true);
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

    public void DisableBar()
    {
        content.gameObject.SetActive(false);
    }
}
