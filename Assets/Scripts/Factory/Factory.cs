using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

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
    [SerializeField] private float _moveTime = 0.5f;
    [SerializeField, NotNull] private GameObject _objectPrefab;

    [Label("Intertact Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private float _interactTime = 0.25f;

    [Label("Factory Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, SearchableEnum] private FactoryType _type;

    [SerializeField] private float _timeToCreate = 0.75f;
    [SerializeField] private int _objectCost = 25;

    private List<FactoryPlate> plates = new List<FactoryPlate>();

    private int _currencyAmount;
    private float _intertactTimer;
    private float _workTimer;

    [Inject] private Player _player;

    public bool IsWorking { get; private set; } = false;


    private void Awake()
    {
        plates.AddRange(GetComponentsInChildren<FactoryPlate>());
    }
    private void Update()
    {
        Transform plate = GetEmptyPlate();

        if (plate == null)
            return;

        if (IsWorking)
        {
            _workTimer += Time.deltaTime;

            if (_workTimer >= _timeToCreate)
            {
                CreatingObject(plate);

                _workTimer = 0;
                IsWorking = false;
            }

            return;
        }

        if (_currencyAmount >= _objectCost)
        {
            IsWorking = true;
            _currencyAmount -= _objectCost;
        }

    }


    public void Interact(Player player)
    {
        _intertactTimer += Time.deltaTime;
        if (_intertactTimer >= _interactTime)
        {
            if (_player.Headquarters.Currencies[CurrencyType.Construction] <= 0)
                return;

            _currencyAmount += 1;
            _player.Headquarters.Currencies[CurrencyType.Construction] -= 1;
            _intertactTimer = 0;
        }
    }
    public void OnEnter(Player player) { }
    public void OnExit(Player player) { }


    private Transform GetEmptyPlate()
    {
        Transform tempTransform = null;
        for (int i = 0; i < plates.Count; i++)
        {
            if (plates[i].HasChild())
                tempTransform = plates[i].content;
        }

        return tempTransform;
    }
    private void CreatingObject(Transform placeTransform)
    {
        if (placeTransform == null)
            return;

        GameObject newObject = Instantiate(_objectPrefab, _spawPoint.position, Quaternion.identity);
        newObject.transform.DOMove(placeTransform.position, _moveTime);
        newObject.transform.parent = placeTransform;
    }
}