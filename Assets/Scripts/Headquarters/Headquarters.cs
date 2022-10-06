using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;
using System.Linq;

[SelectionBase]
public class Headquarters : MonoBehaviour, IInteractable
{
    [SerializeField, DisableInPlayMode, Range(1, 1000)] private float _health;
    [Label("Data Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]

    [Label("View Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private RectTransform _interactPanelPrefab;
    [SerializeField] private CustomButton _customButtonPrefab;
    [SerializeField] private UpgradeButton _upgradeButtonPrefab;
    [SerializeField] private GameObject _headquartersBody;

    [Label("Points Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] protected Transform _dronePoint;
    [SerializeField] protected Transform _finishPoint;

    [Label("Animation Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private DamageAnimationSettings _damageAnimation;

    [Label("Helpers Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Helper _helperPrefab;
    [SerializeField] private int _helperPrice = 0;

    [Inject] private DiContainer _container;
    [Inject] private Canvas _canvas;
    [Inject] private Game _game;

    private HPBar _hpBar;
    private RectTransform _interactGroup;
    private List<CustomButton> _upgradeButtons = new List<CustomButton>();

    public bool IsDead => _health <= 0;
    public Transform DronePoint => _dronePoint;
    public Transform FinishPoint => _finishPoint;

    public static event Action OnDeath;

    private void Awake()
    {
        _game.SetHeadquarters(this);
        _hpBar = GetComponentInChildren<HPBar>();

        if (_hpBar != null)
            _hpBar.InitializationBar(_health);
    }
    private void Start()
    {
        _interactGroup = Instantiate(_interactPanelPrefab, _canvas.transform);

        OpenViewGroup();
        ClosedViewGroup();
    }

    public void ApplyDamage(float damage)
    {
        _health -= damage;

        if (_hpBar != null)
            _hpBar.ChangeValue(_health);

        if (_headquartersBody != null)
            _headquartersBody.transform.DOShakeScale(_damageAnimation.Duration, _damageAnimation.Strenght, _damageAnimation.Vibrato, _damageAnimation.Random);

        if (_health <= 0)
        {
            if (_hpBar != null)
                _hpBar.DisableBar();

            OnDeath?.Invoke();

            this.enabled = false;
        }
    }

    #region Buttons
    private void InitializationButtons()
    {
        if (_game.Data.UpgradesInfo == null)
            return;

        foreach (var upgrade in _game.Data.UpgradesInfo.Upgrades)
        {
            if (upgrade != null)
            {
                UpgradeButton button = Instantiate(_upgradeButtonPrefab, _interactGroup.transform);
                button.Initialization(_game.Data, upgrade.Type, upgrade.Type.ToString(), TryUpgade);
                _upgradeButtons.Add(button);
            }
        }

        CustomButton buyButton = Instantiate(_customButtonPrefab, _interactGroup.transform);
        buyButton.Initialization("Buy Helper", _helperPrice, BuyHelper);
        _upgradeButtons.Add(buyButton);
    }
    private void BuyHelper()
    {
        if(_game.Data.User.TryWithdrawCurrency(CurrencyType.Construction, _helperPrice))
        {
            _container.InstantiatePrefab(_helperPrefab, DronePoint.transform.position, Quaternion.identity, null);
        }
    }
    private void TryUpgade(UpgradeType type)
    {
        UpgradeList upgradeList = _game.Data.UpgradesInfo.Upgrades.Where(x => x.Type == type).First();
        int upgradeIndex = _game.Data.User.UpgradesProgress[type];

        if(_game.Data.User.TryWithdrawCurrency(CurrencyType.Upgrade, upgradeList.Elements[upgradeIndex + 1].Cost))
        {
            _game.Data.User.UpdateUpgradeProgress(type, upgradeIndex + 1);
        }

    }

    #endregion

    #region View UI
    public void OpenViewGroup()
    {
        InitializationButtons();
        _interactGroup.gameObject.SetActive(true);
    }
    public void ClosedViewGroup()
    {
        _interactGroup.gameObject.SetActive(false);

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
