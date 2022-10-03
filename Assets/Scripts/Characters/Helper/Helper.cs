using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Zenject;

public class Helper : MonoBehaviour
{
    [Label("Inventory Settings", skinStyle: SkinStyle.Box)]
    [SerializeField] private int _maxAmmo = 10;

    [Label("Transforms", skinStyle: SkinStyle.Box)]
    [SerializeField] private Transform[] propellers;
    [SerializeField] private GameObject body;
    [SerializeField, NotNull] private Transform _inventory;

    [Label("Visual Settings", skinStyle: SkinStyle.Box)]
    [SerializeField] private float duration = 1;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float _riseTime = 0.5f;
    [SerializeField] private float _liftingHeight = 2f;

    [Inject] private Game _game;
    private bool _fansIsActive = false;
    private Stack<Ammunition> _inventoryAmmo = new Stack<Ammunition>();

    public HelperStateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public bool InventoryFull => _inventoryAmmo.Count == _maxAmmo;
    public bool InventoyEmpty => _inventoryAmmo.Count == 0;
    public bool IsMove => Agent.velocity.sqrMagnitude > 0f;
    public Game Game => _game;

    public static List<Ammunition> TargetAmmunitions = new List<Ammunition>();
    public static List<BaseTurret> TargetTurrets = new List<BaseTurret>();

    public static List<Helper> Helpers { get; private set; } = new List<Helper>();
    public static event Action<Helper> OnHelperCreated;
    public static event Action<Helper> OnHelperDestroyed;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        StateMachine = new HelperStateMachine(this);

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
        if (_inventoryAmmo.Count >= _maxAmmo)
        {
            if (other.CompareTag("Turret"))
            {
                if (other.TryGetComponent(out BaseTurret turret))
                {
                    if (!turret.CanCharge)
                        return;

                    body.transform.DOLocalMoveY(1f, _riseTime).OnComplete(() =>
                    {
                        while (_inventoryAmmo.Count > 0 && turret.CanCharge)
                        {
                            Destroy(_inventoryAmmo.Pop().gameObject);
                            turret.Charge();
                        }
                    });
                }
            }

            return;
        }


        if (other.CompareTag("Ammunition"))
        {
            if (other.TryGetComponent(out Ammunition ammunition))
            {
                if (!ammunition.enabled)
                    return;

                body.transform.DOLocalMoveY(0.25f, _riseTime).OnComplete(() =>
                {

                    ammunition.transform.SetParent(_inventory);
                    ammunition.transform.localPosition = Vector3.zero;

                    ammunition.enabled = false;
                    _inventoryAmmo.Push(ammunition);
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

    public Ammunition GetTargetAmmunition()
    {
        for (int x = 0; x < Factory.Factories.Count; x++)
        {
            Factory factory = Factory.Factories[x];

            for (int i = 0; i < factory.Plates.Count; i++)
            {
                FactoryPlate plate = factory.Plates[i];

                if (plate.gameObject.activeSelf && plate.CanPlace() == false)
                {
                    if (plate.Content.TryGetComponent(out Ammunition ammunition))
                    {
                        if (!TargetAmmunitions.Contains(ammunition))
                            return ammunition;
                    }
                }
            }
        }

        return null;
    }


    public BaseTurret GetTargetTurret()
    {
        int minAmmo = int.MaxValue;

        BaseTurret targetTurret = null;

        for (int i = 0; i < BaseTurret.Turrets.Count; i++)
        {
            BaseTurret turret = BaseTurret.Turrets[i];

            if (!TargetTurrets.Contains(turret) && turret.gameObject.activeSelf && turret.CanCharge && turret.enabled)
            {
                if (turret.Ammo <= minAmmo)
                {
                    targetTurret = turret;
                }
            }
        }

        return targetTurret;
    }
}
