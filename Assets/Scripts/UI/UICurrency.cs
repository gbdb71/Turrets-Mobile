using UnityEngine;
using TMPro;
using Zenject;

public class UICurrency : MonoBehaviour
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private TextMeshProUGUI amountText;
    [Inject] private Game _game;

    private void Start()
    {
        UserData.OnCurrencyChanged += UpdateUI;

        UpdateUI(currencyType, _game.Data.User.Currencies[currencyType]);
    }

    private void UpdateUI(CurrencyType type, int amount)
    {
        if (type == currencyType)
            amountText.text = amount.ToString();
    }
}
