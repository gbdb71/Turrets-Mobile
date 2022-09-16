using UnityEngine;
using Zenject;
using TMPro;

public class UICurrency : MonoBehaviour
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private TextMeshProUGUI amountText;

    private void Start()
    {
        Headquarters.OnCurrencyChanged += UpdateUI;
    }

    private void UpdateUI(CurrencyType type, int amount)
    {
        if (type == currencyType)
            amountText.text = amount.ToString();
    }
}
