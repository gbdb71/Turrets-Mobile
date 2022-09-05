using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class Factory : MonoBehaviour, IInteractable
{
    [Header("Other Settings")]
    private List<FactoryPlate> plates = new List<FactoryPlate>();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float moveTime = 0.5f;

    [Header("View Settings")]
    [SerializeField] private float interactTime = 0.25f;

    [Header("Create Settings")]
    [SerializeField] private GameObject _objectPrefab;
    public FactoryType factoryType;
    public enum FactoryType
    {
        Ammunition,
        Turrets
    }

    [SerializeField] private float _timeToCreate = 0.75f;
    [SerializeField] private int _objectCost = 25;

    private int _currencyAmount;
    private float _intertactTimer;
    private float _workTimer;

    [Inject] private Player _player;

    public bool IsWorking { get; private set; } = false;

    public void Awake()
    {
        plates.AddRange(GetComponentsInChildren<FactoryPlate>());
    }

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

        GameObject newObject = Instantiate(_objectPrefab, spawnPoint.position, Quaternion.identity);
        newObject.transform.DOMove(placeTransform.position, moveTime);
        newObject.transform.parent = placeTransform;
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
        if (_intertactTimer >= interactTime)
        {
            if (_player.Headquarters.ConstructionCurrency <= 0)
                return;

            _currencyAmount += 1;
            _player.Headquarters.ConstructionCurrency -= 1;
            _intertactTimer = 0;
        }
    }

    public void OnEnter(Player player) { }
    public void OnExit(Player player) { }
}