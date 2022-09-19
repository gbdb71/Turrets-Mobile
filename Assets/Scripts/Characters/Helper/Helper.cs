using System;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Zenject;

public class Helper : MonoBehaviour
{
    [SerializeField] private int _maxAmmo = 10;
    
    [SerializeField] private bool FansIsActive = false;

    [SerializeField] private GameObject contentObject;
    [SerializeField] private Transform[] propellers;
    [SerializeField] private GameObject body;

    [Inject]
    private Game _game;
    private int _currentAmmo = 0;
    
    public HelperStateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public bool InventoryFull => _currentAmmo == _maxAmmo;
    public bool InventoyEmpty => _currentAmmo <= 0;
    public bool IsMove => Agent.velocity != Vector3.zero;
    public Game Game => _game;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        StateMachine = new HelperStateMachine(this);
        contentObject.SetActive(false);
    }

    private void Update()
    {
        Agent.DOPause();

        StateMachine.Update();

        if (IsMove && !FansIsActive)
            RunTheFans();

        if (!IsMove && FansIsActive)
            TurnOffTheFans();
    }

    [SerializeField] float duration = 1;
    [SerializeField] Vector3 rotation;

    [SerializeField] private float riseTime = 0.5f;
    [SerializeField] private float liftingHeight = 2f;

    [ContextMenu("Run")]
    public void RunTheFans()
    {
        FansIsActive = true;
        Debug.Log("Run");

        for (int i = 0; i < propellers.Length; i++)
        {
            propellers[i].DOLocalRotate(rotation, duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }

        body.transform.DOLocalMoveY(liftingHeight, riseTime);
    }

    [ContextMenu("End")]
    public void TurnOffTheFans()
    {
        FansIsActive = false;
        Debug.Log("Turn");

        body.transform.DOLocalMoveY(0, riseTime).OnComplete(() =>
        {
            for (int i = 0; i < propellers.Length; i++)
            {
                propellers[i].DOKill();
            }
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currentAmmo >= _maxAmmo)
            return;

        if (other.CompareTag("Ammunition"))
        {
            if (other.TryGetComponent(out Ammunition ammunition))
            {
                body.transform.DOLocalMoveY(0.25f, riseTime).OnComplete(() =>
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
                body.transform.DOLocalMoveY(1f, riseTime).OnComplete(() =>
                {
                    turret.Charge();
                    _currentAmmo--;
                });
            }
        }
    }
}
