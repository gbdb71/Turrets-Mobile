using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BaseFactory : MonoBehaviour
{
    [Inject]
    private Player _player;

    [SerializeField] private int _objectCost;
    public int ObjectCost { set { _objectCost = Mathf.Clamp(value, 0, 999); } }
    [SerializeField] private int _costToCreate;

    [Header("Other Settings")]
    [SerializeField] private bool _platformIsActive = false;
    // [SerializeField] private bool _readyToCreate = false;
    [SerializeField] private float _timeToCreate = 0.75f;

    //[Range(1, 99)]
    //[SerializeField] private int _pointsToCreate;
    //[SerializeField] private int _currentPointsToCreate;

    [SerializeField] private int _maxWarehouseCapacity = 1;
    [SerializeField] private int _currentWarehouseCapacity = 0;

    public List<FactoryPlate> plates = new List<FactoryPlate>();

    [Header("View Settings")]
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillTime = 0.25f;

    [Header("Create Settings")]
    [SerializeField] private GameObject _objectPrefab;

    public void Awake()
    {
        plates.AddRange(GetComponentsInChildren<FactoryPlate>());
        _costToCreate = _objectCost;
    }

    private IEnumerator Creating()
    {
        yield return new WaitForSeconds(_timeToCreate);

    }

    private Transform CheckFactoryPlates()
    {
        Transform tempTransform = null;
        foreach (FactoryPlate plate in plates)
            if (plate.CheckChild())
                tempTransform = plate.content;

        return tempTransform;
    }

    public bool CheckReadinessForUse()
    {
        bool isReady = false;
        if (!_platformIsActive)
            return false;

        if (CheckFactoryPlates() == null)
            return false;

        if (_player.Headquarters.ConstructionCurrency! > 0)
            return false;

        return true;
    }

    public void CreatingObject(Transform placeTransform)
    {
        if (placeTransform == null)
            return;
        
        GameObject newObject = Instantiate(_objectPrefab, placeTransform.position, Quaternion.identity);
        newObject.transform.parent = placeTransform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            _platformIsActive = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CheckReadinessForUse();

        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            if (_costToCreate <= 0) 
            {
                CreatingObject(CheckFactoryPlates());
            }
            else
            {
                _costToCreate -= 1;
                _player.Headquarters.ConstructionCurrency -= 1;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            _platformIsActive = false;
        }
    }
}