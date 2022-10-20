using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FactoryView : BaseView
{
    private Factory _factory;
    private int _cost;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI currentValueText;

    public void InitializationPanel(Sprite iconSprite, int cost, Factory factory)
    {
        _factory = factory;
        iconImage.sprite = iconSprite;
        _cost = cost;

    }

    protected override void UpdateLogic()
    {
        if (_factory == null)
            return;

        currentValueText.text = $"{_factory.CurrencyAmount.ToString("D2")} / {_cost}";
    }
}
