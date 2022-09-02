using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BaseFactory : MonoBehaviour
{
    [Inject]
    private Player _player;

    [SerializeField] private int _objectCost;
    public int ObjectCost { set { _objectCost = Mathf.Clamp(value, 0, 999); } }

    [Header("Other Settings")]
    [SerializeField] private bool _platformIsActive = false;
    [SerializeField] private float _timeToCreate = 0.75f;

    [Range(1, 99)]
    [SerializeField] private int _pointsToCreate;
    [SerializeField] private int _currentPointsToCreate;

    [SerializeField] private int _maxWarehouseCapacity = 1;
    [SerializeField] private int _currentWarehouseCapacity = 0;
    [SerializeField] private Transform warehousPosition;

    [Header("View Settings")]
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillTime = 0.25f;

    [Header("Create Settings")]
    [SerializeField] private GameObject _turretPrefab;
    
    private IEnumerator CreatingTurret()
    {
        yield return new WaitForSeconds(_timeToCreate);

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
        if (!_platformIsActive)
            return;

        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {

            if(_currentPointsToCreate <= 0)
            {

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
