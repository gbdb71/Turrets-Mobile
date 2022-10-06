using TMPro;
using UnityEngine;
using Zenject;

public class MoneyAbillity : BaseAbillity
{
    [SerializeField, Range(30, 700)] private int _amount;
    [Inject] private Data _data;

    private TextMeshProUGUI _text;

    protected override void Awake()
    {
        base.Awake();

        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.text = $"Money x{_amount}";
    }

    protected override void Activate()
    {
        base.Activate();

        _data.User.TryAddCurrency(CurrencyType.Construction, _amount);
    }

    public override void Clear()
    {
    }
}

