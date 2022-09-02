using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BaseFactory : MonoBehaviour, IInteractable
{
    [Header("Other Settings")]
    private List<FactoryPlate> plates = new List<FactoryPlate>();

    [Header("View Settings")]
    [SerializeField] private Image fillImage;
    [SerializeField] private float interactTime = 0.25f;

    [Header("Create Settings")]
    [SerializeField] private GameObject _objectPrefab;

    [SerializeField] private float _timeToCreate = 0.75f;
    [SerializeField] private int _objectCost = 25;

    private int curentCurrency;
    private float timer;

    [Inject]
    private Player _player;

    //public int ObjectCost { set { _objectCost = Mathf.Clamp(value, 0, 999); } }

    public void Awake()
    {
        plates.AddRange(GetComponentsInChildren<FactoryPlate>());
    }

    private IEnumerator Creating()
    {
        yield return new WaitForSeconds(_timeToCreate);
        coroutine = null;
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

    private bool CheckReadinessForUse()
    {
        if (GetEmptyPlate() == null)
            return false;

        return true;
    }

    Coroutine coroutine;

    private void CreatingObject(Transform placeTransform)
    {
        if (placeTransform == null)
            return;

        GameObject newObject = Instantiate(_objectPrefab, placeTransform.position, Quaternion.identity);
        newObject.transform.parent = placeTransform;
    }

    private void Update()
    {
       // if(coroutine == null)
    }

    public void Interact(Player player)
    {
        //if (!CheckReadinessForUse())
        //    return;

        //if (_costToCreate <= 0)
        //{
        //    _costToCreate = _objectCost;
        //    CreatingObject(GetEmptyPlate());
        //    return;
        //}
        //else
        //{
            timer += Time.deltaTime;
            if (timer >= interactTime)
            {
                if (_player.Headquarters.ConstructionCurrency <= 0)
                    return;

                curentCurrency += 1;
                _player.Headquarters.ConstructionCurrency -= 1;
                timer = 0;
            }

        //}
    }

    public void OnEnter(Player player) { }
    public void OnExit(Player player) { }
}