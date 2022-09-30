using System;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Zenject;

public class Helper : MonoBehaviour
{
    [Label("Inventory Settings", skinStyle: SkinStyle.Box)]
    [SerializeField] private int _maxAmmo = 10;

    [Label("Transforms", skinStyle: SkinStyle.Box)]
    [SerializeField] private GameObject contentObject;
    [SerializeField] private Transform[] propellers;
    [SerializeField] private GameObject body;

    [Label("Visual Settings", skinStyle: SkinStyle.Box)]
    [SerializeField] private float duration = 1;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float _riseTime = 0.5f;
    [SerializeField] private float _liftingHeight = 2f;

    [Inject]
    private Game _game;
    private int _currentAmmo = 0;
    private bool _fansIsActive = false;

    public HelperStateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public bool InventoryFull => _currentAmmo == _maxAmmo;
    public bool InventoyEmpty => _currentAmmo <= 0;
    public bool IsMove => Agent.velocity != Vector3.zero;
    public Game Game => _game;

    public static List<Helper> Helpers { get; private set; } = new List<Helper>();
    public static event Action<Helper> OnHelperCreated;
    public static event Action<Helper> OnHelperDestroyed;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        StateMachine = new HelperStateMachine(this);
        contentObject.SetActive(false);

        Helpers.Add(this);
        OnHelperCreated?.Invoke(this);
    }
    private void Update()
    {
        Agent.DOPause();

        StateMachine.Update();

        if (IsMove && !_fansIsActive)
            RunTheFans();

        if (!IsMove && _fansIsActive)
            TurnOffTheFans();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_currentAmmo >= _maxAmmo)
            return;

        if (other.CompareTag("Ammunition"))
        {
            if (other.TryGetComponent(out Ammunition ammunition))
            {
                body.transform.DOLocalMoveY(0.25f, _riseTime).OnComplete(() =>
                {
                    contentObject.SetActive(true);
                    _currentAmmo++;

                    //REMOVE: 
                    Destroy(other.gameObject);
                });
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_currentAmmo <= 0)
        {
            contentObject.gameObject.SetActive(false);
            return;
        }

        if (other.CompareTag("Turret"))
        {
            if (other.TryGetComponent(out BaseTurret turret))
            {
                body.transform.DOLocalMoveY(1f, _riseTime).OnComplete(() =>
                {
                    turret.Charge();
                    _currentAmmo--;
                });
            }
        }
    }
    private void OnDestroy()
    {
        Helpers.Remove(this);
        OnHelperDestroyed?.Invoke(this);
    }

    public void RunTheFans()
    {
        _fansIsActive = true;

        for (int i = 0; i < propellers.Length; i++)
        {
            propellers[i].DOLocalRotate(rotation, duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }

        body.transform.DOLocalMoveY(_liftingHeight, _riseTime);
    }
    public void TurnOffTheFans()
    {
        _fansIsActive = false;

        body.transform.DOLocalMoveY(0, _riseTime).OnComplete(() =>
        {
            for (int i = 0; i < propellers.Length; i++)
            {
                propellers[i].DOKill();
            }
        });
    }

}
