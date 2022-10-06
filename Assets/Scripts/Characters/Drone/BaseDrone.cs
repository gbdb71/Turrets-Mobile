using Assets.Scripts.StateMachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseDrone<T> : MonoBehaviour where T : StateMachine
{
    [Label("Transforms", skinStyle: SkinStyle.Box)]
    [SerializeField] protected Transform[] propellers;
    [SerializeField] protected GameObject body;

    [Label("Visual Settings", skinStyle: SkinStyle.Box)]
    [SerializeField] protected float duration = 1;
    [SerializeField] protected Vector3 rotation;
    [SerializeField] protected float _riseTime = 0.5f;
    [SerializeField] protected float _liftingHeight = 2f;
    [SerializeField] protected bool _canLifting = true;

    protected bool _fansIsActive = false;

    public NavMeshAgent Agent { get; protected set; }
    public bool IsMove => Agent.velocity.sqrMagnitude > 0f;
    public T StateMachine { get; protected set; }

    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (!_canLifting)
            body.transform.DOLocalMoveY(_liftingHeight, _riseTime);
    }

    private void Update()
    {
        Agent.DOPause();

        StateMachine.Update();

        if (IsMove && !_fansIsActive)
            RunTheFans();

        if (!IsMove && _fansIsActive)
            TurnOffTheFans();

        UpdateLogic();
    }

    protected virtual void UpdateLogic() { }

    protected void RunTheFans()
    {
        _fansIsActive = true;

        for (int i = 0; i < propellers.Length; i++)
        {
            propellers[i].DOLocalRotate(rotation, duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }

        if (_canLifting)
            body.transform.DOLocalMoveY(_liftingHeight, _riseTime);
    }

    protected void TurnOffTheFans()
    {
        _fansIsActive = false;

        if (_canLifting)
        {
            body.transform.DOLocalMoveY(0, _riseTime).OnComplete(() =>
            {
                for (int i = 0; i < propellers.Length; i++)
                {
                    propellers[i].DOKill();
                }
            });
        }
    }

}
