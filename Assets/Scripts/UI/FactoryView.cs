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
        _factory = factory;
        iconImage.sprite = iconSprite;
        _cost = cost;

        fillImage.fillAmount = 0;
    }

    private void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);

        if (_factory == null)
            return;

        fillImage.fillAmount = _factory.CreateProgress;
        currentValueText.text = $"{_factory.CurrencyAmount.ToString("D2")} / {_cost}";
}
}
