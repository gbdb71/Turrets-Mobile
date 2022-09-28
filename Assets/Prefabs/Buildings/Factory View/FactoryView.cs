using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Cinemachine;

public class FactoryView : MonoBehaviour
{
    private Factory _factory;
    private int _cost;

    [Inject] private CinemachineVirtualCamera _playerCamera;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI currentValueText;

    public void InitializationPanel(Sprite iconSprite, int cost, Factory factory)
    {
        Debug.Log("Initialization");
        _factory = factory;
        iconImage.sprite = iconSprite;
        _cost = cost;

        fillImage.fillAmount = 0;
        currentValueText.text = 00 + " / "  + cost.ToString();
    }

    private void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);

        if (_factory == null)
            return;

        string leftString;
        int value = _factory.ViewValue;

        if (value < 10)
            leftString = 0 + value.ToString();
        else
            leftString = value.ToString();

        fillImage.fillAmount = _factory.ViewFill;
        currentValueText.text = leftString + " / " + _cost.ToString(); 
    }
}
