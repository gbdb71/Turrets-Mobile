using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;
using UnityEngine.UI;

public enum FactoryType
{
    Ammunition,
    Turrets
}

[SelectionBase]
public class Factory : MonoBehaviour, IInteractable
{
    [Label("Spawning Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] private Transform _spawPoint;
    [SerializeField, NotNull] private BaseTurret _turretPrefab;

    [Label("Intertact Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private float _interactTime = 0.25f;

    [Label("Factory Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, SearchableEnum] private FactoryType _type;

    [SerializeField] private int _objectCost = 25;

    private List<TurretPlace> _plates = new List<TurretPlace>();
    private int _currencyAmount;
    private float _intertactTimer;
    [Inject] private GameLogic _game;

    public List<TurretPlace> Plates => _plates;
    public FactoryType Type => _type;
    public static List<Factory> Factories { get; private set; } = new List<Factory>();
    public Sprite plateSprite;
    public int CurrencyAmount => _currencyAmount;

    private void Awake()
    {
        Factories.Add(this);

        _plates.AddRange(GetComponentsInChildren<TurretPlace>());

        FactoryView _factoryView = GetComponentInChildren<FactoryView>();

        if (_factoryView != null)
            _factoryView.InitializationPanel(plateSprite, _objectCost, this);
    }

    private void Update()
    {
        TurretPlace place = GetEmptyPlate();

        if (place == null)
            return;

        if (_currencyAmount >= _objectCost)
        {
            CreatingObject(place);
            _currencyAmount -= _objectCost;
        }

    }
    private void OnDestroy()
    {
        Factories.Remove(this);
    }

    public void Interact(Player player)
    {
        _intertactTimer += Time.deltaTime;

        if (_intertactTimer >= _interactTime)
        {
            if (_game.Data.User.TryWithdrawCurrency(CurrencyType.Construction, 1))
            {
                _currencyAmount += 1;

                if (_currencyAmount % _objectCost == 0)
                {
                    _intertactTimer = -.6f;
                    
                }
                else
                    _intertactTimer = 0;
            }
        }
    }

    public void OnEnter(Player player) { }
    public void OnExit(Player player) { }

    private TurretPlace GetEmptyPlate()
    {
        for (int i = 0; i < _plates.Count; i++)
        {
            if (!_plates[i].HasTurret)
                return _plates[i];
        }

        return null;
    }

    private void CreatingObject(TurretPlace place)
    {
        BaseTurret newObject = Instantiate(_turretPrefab, _spawPoint.position, Quaternion.identity);
        place.Place(newObject);
    }
}