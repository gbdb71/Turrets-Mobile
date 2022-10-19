
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public enum CurrencyType
{
    Construction
}


[System.Serializable]
public class UserData : INotifyPropertyChanged
{
    private Dictionary<CurrencyType, int> _currencies { get; set; }
    private int _currentLevel;

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            if (_currentLevel != value)
            {
                _currentLevel = value;
                NotifyPropertyChanged(nameof(CurrentLevel));
            }
        }
    }
    public IReadOnlyDictionary<CurrencyType, int> Currencies => _currencies;
    public static event Action<CurrencyType, int> OnCurrencyChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public UserData()
    {
        _currencies = new Dictionary<CurrencyType, int>();

        var currenciesTypes = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();

        foreach (var cType in currenciesTypes)
        {
            _currencies.Add(cType, 0);
        }

        NotifyPropertyChanged(nameof(_currencies));
    }

    public void TryAddCurrency(CurrencyType type, int amount)
    {
        if (amount > 0)
        {
            if (_currencies.ContainsKey(type))
                _currencies[type] += amount;
            else
                _currencies.Add(type, amount);

            OnCurrencyChanged?.Invoke(type, _currencies[type]);
            NotifyPropertyChanged(nameof(_currencies));
        }
    }
    public bool TryWithdrawCurrency(CurrencyType type, int amount)
    {
        if (_currencies.ContainsKey(type) && _currencies[type] >= amount)
        {
            _currencies[type] -= amount;

            if (_currencies[type] < 0)
            {
                _currencies[type] = 0;
            }

            OnCurrencyChanged?.Invoke(type, _currencies[type]); return true;
            NotifyPropertyChanged(nameof(_currencies));
        }

        return false;
    }
    public void SetCurrencyValue(CurrencyType type, int value)
    {
        if (value >= 0)
        {
            _currencies[type] = value;
            OnCurrencyChanged?.Invoke(type, _currencies[type]);
            NotifyPropertyChanged(nameof(_currencies));

        }
    }

    private void NotifyPropertyChanged(string info)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}

