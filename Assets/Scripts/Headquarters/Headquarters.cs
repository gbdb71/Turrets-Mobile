using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Headquarters : MonoBehaviour
{
    [Header("DATA Settings")]
    [SerializeField] private UpgradesInfo _upgradeInfo;
    [SerializeField] private List<UpgradeButton> _upgradeButtons = new List<UpgradeButton>();

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

                string typeName = nameof(type);

                int upgrade = int.Parse(_data[typeName]);
                _upgradeButtons[i].Initialization(_upgradeInfo.upgrades[i], upgrade, this);
            }
            else
            {
                _upgradeButtons[i].gameObject.SetActive(false);
                break;
            }
        }
    }


    public void ValuePassing(UpgradeList.UpgradeType type, float value)
    {
        string key = nameof(type);

        if (_data.ContainsKey(key))
        {
            _data[key] = value.ToString();
        }
        else
        {
            _data.Add(key, value.ToString());
        }

        switch (type)
        {
            case UpgradeList.UpgradeType.Speed:
                _player.Movement.Speed = value;
                break;

            case UpgradeList.UpgradeType.SpeedWithTurret:
                _player.Movement.SpeedWithTurret = value;
                break;

            case UpgradeList.UpgradeType.AmmoCount:

                break;

        }

        SaveData();
    }

    #endregion

    #region Data

    private void SaveData()
    {
        if (_data != null)
        {
            foreach (var upgrade in _data)
            {
                PlayerPrefs.SetString(nameof(upgrade.Key), upgrade.Value.ToString());
            }
        }

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
