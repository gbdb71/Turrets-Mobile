using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;
using Characters.Drone.Helper;

public class HelperDrone : BaseDrone<HelperStateMachine>
{
    [Label("Inventory Settings", skinStyle: SkinStyle.Box)]
    [SerializeField] private int _maxAmmo = 10;
    [SerializeField, NotNull] protected Transform _inventory;

    [Inject] private Game _game;
    private Stack<Ammunition> _inventoryAmmo = new Stack<Ammunition>();

    public bool InventoryFull => _inventoryAmmo.Count == _maxAmmo;
    public bool InventoyEmpty => _inventoryAmmo.Count == 0;
    public Game Game => _game;

    public static List<Ammunition> TargetAmmunitions = new List<Ammunition>();
    public static List<BaseTurret> TargetTurrets = new List<BaseTurret>();

    public static List<HelperDrone> Helpers { get; private set; } = new List<HelperDrone>();
    public static event Action<HelperDrone> OnHelperCreated;
    public static event Action<HelperDrone> OnHelperDestroyed;

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new HelperStateMachine(this);
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
