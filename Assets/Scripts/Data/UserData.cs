
using System;
using System.Collections.Generic;
using System.Linq;

public enum CurrencyType
{
    Upgrade,
    Construction
}


[System.Serializable]
public class UserData
{
    public Dictionary<CurrencyType, int> Currencies {get; set;}
    public Dictionary<UpgradeType, int> UpgradesProgress { get; set;}


    public static event Action<UpgradeType, int> OnUpgradeChanged;
    public static event Action<CurrencyType, int> OnCurrencyChanged;

    public UserData()
    {
        Currencies = new Dictionary<CurrencyType, int>();
        UpgradesProgress = new Dictionary<UpgradeType, int>();


        var currenciesTypes = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();
        var upgradeTypes = Enum.GetValues(typeof(UpgradeType)).Cast<UpgradeType>();

        foreach (var cType in currenciesTypes)
        {
            Currencies.Add(cType, 0);
        }

        foreach (var uType in upgradeTypes)
        {
            UpgradesProgress.Add(uType, 0);
        }
    }

    public void TryAddCurrency(CurrencyType type, int amount)
    {
        if (amount > 0)
        {
            if (Currencies.ContainsKey(type))
               Currencies[type] += amount;
            else
                Currencies.Add(type, amount);

            OnCurrencyChanged?.Invoke(type, Currencies[type]);
        }
    }
    public bool TryWithdrawCurrency(CurrencyType type, int amount)
    {
        if (Currencies.ContainsKey(type) && Currencies[type] >= amount)
        {
            Currencies[type] -= amount;

            if (Currencies[type] < 0)
            {
                Currencies[type] = 0;
            }

            OnCurrencyChanged?.Invoke(type, Currencies[type]);
            return true;
        }

        return false;
    }

    public void SetCurrencyValue(CurrencyType type, int value)
    {
        if(value >= 0)
        {
            Currencies[type] = value;
            OnCurrencyChanged?.Invoke(type, Currencies[type]);
        }
    }

    public void UpdateUpgradeProgress(UpgradeType type, int value)
    {
        UpgradesProgress[type] = value;

        OnUpgradeChanged?.Invoke(type, value);
    }
}

