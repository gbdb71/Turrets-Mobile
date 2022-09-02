using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Headquarters : MonoBehaviour
{
    [Header("DATA Settings")]
    [SerializeField] private UpgradesInfo _upgradeInfo;
    [SerializeField] private List<UpgradeButton> _upgradeButtons = new List<UpgradeButton>();

    [SerializeField] private int _upgradeCurrency = 0;
    [SerializeField] private int _constructionCurrency = 0;
    public int ConstructionCurrency { get => _constructionCurrency; set { _constructionCurrency = Mathf.Clamp(value, 0, 9999); } }
    
    [Header("View Settings")]
    [SerializeField] private CanvasGroup _сanvasGroup;

    [Inject]
    private Player _player;

    private Dictionary<string, string> _data = new Dictionary<string, string>();

    public void Start()
    {
        LoadData();

        ClosedViewGroup();
        InitializationButtons();
    }

    [ContextMenu("Add Currency")]
    public void AddCurrency()
    {
        _upgradeCurrency += 100;
        _constructionCurrency += 100;
    }

    #region Buttons
    private void InitializationButtons()
    {
        if (_upgradeInfo == null)
        {
            for (int i = 0; i < _upgradeButtons.Count; i++)
            {
                _upgradeButtons[i].gameObject.SetActive(false);
            }
            return;
        }

        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            if (i < _upgradeInfo.upgrades.Count && _upgradeInfo.upgrades[i] != null)
            {
                UpgradeList.UpgradeType type = _upgradeInfo.upgrades[i].Type;

                int index = int.Parse(_data[type.ToString()]);
                _upgradeButtons[i].Initialization(_upgradeInfo.upgrades[i], index, this);

                ValuePassing(type, _upgradeInfo.upgrades[i].elementList[index].Value, index);
            }
            else
            {
                _upgradeButtons[i].gameObject.SetActive(false);
                break;
            }
        }
    }


    public void ValuePassing(UpgradeList.UpgradeType type, float value, int index)
    {
        string key = type.ToString();

        _data[key] = index.ToString();

        switch (type)
        {
            case UpgradeList.UpgradeType.Speed:
                _player.Movement.Speed = value;
                break;

            case UpgradeList.UpgradeType.SpeedWithTurret:
                _player.Movement.SpeedWithTurret = value;
                break;

            case UpgradeList.UpgradeType.AmmoCount:
                _player.Inventory.AmmoCount = (int)value;
                break;

        }

        SaveData();
    }

    #endregion

    #region Data

    private void SaveData()
    {
        if (_data != null)
            foreach (var itemData in _data)
                PlayerPrefs.SetString(itemData.Key.ToString(), itemData.Value.ToString());

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        _data = new Dictionary<string, string>();

        foreach (string name in Enum.GetNames(typeof(UpgradeList.UpgradeType)))
        {
            string value = PlayerPrefs.HasKey(name) ? PlayerPrefs.GetString(name) : "0";

            _data.Add(name, value);
        }
    }

    [ContextMenu("Clear Data")]
    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

    #region View UI
    public void OpenViewGroup()
    {
        _сanvasGroup.alpha = 1;
    }

    public void ClosedViewGroup()
    {
        _сanvasGroup.alpha = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            if (!player.Inventory.HasTurret)
                OpenViewGroup();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            ClosedViewGroup();
        }
    }
    #endregion
}
