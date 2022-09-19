using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

public enum CurrencyType
{
    Upgrade,
    Construction
}

[SelectionBase]
public class Headquarters : MonoBehaviour, IInteractable
{
    [SerializeField, DisableInPlayMode, Range(100, 1000)] private float _health;
    [Label("Data Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private UpgradesInfo _upgradeInfo;
    [SerializeField, EditorButton(nameof(ClearData), "ClearData"), DisableInPlayMode] private SerializedDictionary<CurrencyType, int> _currencies;

    [Label("View Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private CanvasGroup _interactGroupPrefab;
    [SerializeField] private CustomButton _upgradeButtonPrefab;
    [SerializeField] private GameObject _headquartersBody;

    [Inject] private Player _player;
    [Inject] private Canvas _canvas;
    [Inject] private Game _game;

    private HPBar _hpBar;
    private CanvasGroup _interactGroup;

    private List<CustomButton> _upgradeButtons = new List<CustomButton>();
    private Dictionary<string, string> _data = new Dictionary<string, string>();
    
    public Dictionary<CurrencyType, int> Currencies => _currencies.BuildNativeDictionary();
    public Transform targetPoint;

    public bool IsDead => _health <= 0;

    public event Action OnDeath;
    public static event Action<CurrencyType, int> OnCurrencyChanged;

    private void Awake()
    {
        _game.SetHeadquarters(this);
        _hpBar = GetComponentInChildren<HPBar>();

        if (_hpBar != null)
            _hpBar.InitializationBar(_health);
    }

    private void Start()
    {
        _interactGroup = Instantiate(_interactGroupPrefab, _canvas.transform);

        LoadData();

        OpenViewGroup();
        ClosedViewGroup();
    }

    [SerializeField] float duration = 1f;
    [SerializeField] float strenght = 15;
    [SerializeField] int vibrato = 10;
    [SerializeField] float random = 25;
    public void ApplyDamage(float damage)
    {
        _health -= damage;

        if (_hpBar != null)
            _hpBar.ChangeValue(_health);

        if (_headquartersBody != null)
            _headquartersBody.transform.DOShakeScale(duration, strenght, vibrato, random);

        if (_health <= 0)
        {
            if (_hpBar != null)
                _hpBar.DisableBar();

            OnDeath?.Invoke();

            //REMOVE
            this.enabled = false;
        }
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

            OnCurrencyChanged?.Invoke(type, _currencies[type]);
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

            OnCurrencyChanged?.Invoke(type, _currencies[type]);
            return true;
        }

        return false;
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

                UpgradeButton button = (UpgradeButton)Instantiate(_upgradeButtonPrefab, _interactGroup.transform);
                button.Initialization(_upgradeInfo.upgrades[i], index, _upgradeInfo.upgrades[i].name, this);
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
        {
            if (_data.ContainsKey("UpgradeCurrency"))
                _data["UpgradeCurrency"] = Currencies[CurrencyType.Upgrade].ToString();
            else
                _data.Add("UpgradeCurrency", Currencies[CurrencyType.Upgrade].ToString());

            foreach (var itemData in _data)
                PlayerPrefs.SetString(itemData.Key.ToString(), itemData.Value.ToString());
        }

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        _data = new Dictionary<string, string>();

        int upgradeValue = PlayerPrefs.HasKey("UpgradeCurrency") ? int.Parse(PlayerPrefs.GetString("UpgradeCurrency")) : 0;
        _currencies[CurrencyType.Upgrade] = upgradeValue;
        OnCurrencyChanged?.Invoke(CurrencyType.Upgrade, upgradeValue);

        int constructionValue = _game.CurrentLevel.ConstructionCurrency;
        _currencies[CurrencyType.Construction] = constructionValue;
        OnCurrencyChanged?.Invoke(CurrencyType.Construction, constructionValue);

        foreach (string name in Enum.GetNames(typeof(UpgradeList.UpgradeType)))
        {
            string value = PlayerPrefs.HasKey(name) ? PlayerPrefs.GetString(name) : "0";

            _data.Add(name, value);
        }

        //OnCurrenciesChanged();
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

    public void Interact(Player player) { }

    public void OnExit(Player player)
    {
        ClosedViewGroup();
    }
    #endregion
}
