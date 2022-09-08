using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum CurrencyType
{
    Upgrade,
    Construction
}
[SelectionBase]
public class Headquarters : MonoBehaviour, IInteractable
{
    [Label("Data Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private UpgradesInfo _upgradeInfo;
    [SerializeField, EditorButton(nameof(ClearData), "ClearData"), DisableInPlayMode] private SerializedDictionary<CurrencyType, int> _currencies;


    [Label("View Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private CanvasGroup _interactGroupPrefab;
    [SerializeField] private UpgradeButton _interactButtonPrefab;

    [Inject] private Player _player;
    [Inject] private Canvas _canvas;

    private CanvasGroup _interactGroup;

    private List<UpgradeButton> _upgradeButtons = new List<UpgradeButton>();
    private Dictionary<string, string> _data = new Dictionary<string, string>();

    public Dictionary<CurrencyType, int> Currencies => _currencies.BuildNativeDictionary();

    private void Awake()
    {
        _player.SetHeadquarters(this);
    }
    private void Start()
    {
        _interactGroup = Instantiate(_interactGroupPrefab, _canvas.transform);

        LoadData();

        ClosedViewGroup();
    }

    #region Currency 

    public void TryAddCurrency(CurrencyType type, int amount)
    {
        if (amount > 0)
        {
            if (_currencies.ContainsKey(type))
                _currencies[type] += amount;
            else
                _currencies.Add(type, amount);
        }
    }
    public void TryWithdrawCurrency(CurrencyType type, int amount)
    {
        if (_currencies.ContainsKey(type))
        {
            _currencies[type] -= amount;

            if (_currencies[type] < 0)
            {
                _currencies[type] = 0;
            }
        }
    }

    #endregion

    #region Buttons
    private void InitializationButtons()
    {
        if (_upgradeInfo == null)
            return;

        for (int i = 0; i < _upgradeInfo.upgrades.Count; i++)
        {
            if (_upgradeInfo.upgrades[i] != null)
            {
                UpgradeList.UpgradeType type = _upgradeInfo.upgrades[i].Type;

                int index = int.Parse(_data[type.ToString()]);

                UpgradeButton button = Instantiate(_interactButtonPrefab, _interactGroup.transform);
                button.Initialization(_upgradeInfo.upgrades[i], index, this);
                _upgradeButtons.Add(button);

                ValuePassing(type, _upgradeInfo.upgrades[i].elementList[index].Value, index);
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

    private void ClearData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    #endregion

    #region View UI
    public void OpenViewGroup()
    {
        InitializationButtons();
        _interactGroup.alpha = 1;
    }

    public void ClosedViewGroup()
    {
        _interactGroup.alpha = 0;

        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            Destroy(_upgradeButtons[i].gameObject);
        }

        _upgradeButtons.Clear();
    }

    public void OnEnter(Player player)
    {
        if (!player.Inventory.HasTurret)
            OpenViewGroup();
    }

    public void Interact(Player player)
    {
    }

    public void OnExit(Player player)
    {
        ClosedViewGroup();
    }
    #endregion

    #region Editor

    private void AddCurrency()
    {

    }

    #endregion
}
