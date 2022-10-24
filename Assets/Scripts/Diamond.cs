using UnityEngine;
using DG.Tweening;
using Zenject;

public class Diamond : MonoBehaviour
{
    [SerializeField] private CurrencyType _currencyType;
    [SerializeField] private float _rotationDuration = 1f;
    [SerializeField] private float _scaleDuration = .7f;

    private Transform _target;
    private Collider _collider;
    public int Amount;
    private bool _activated;


    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 360, 0), _rotationDuration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
    }

    private void Update()
    {
        if (!_activated)
            return;

        Vector3 targetPos = _target.position;

        if (Vector3.Distance(transform.position, targetPos) < .8f)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, _scaleDuration / 8);
    }

    public void Activate(Transform target)
    {
        if (_activated)
            return;

        _activated = true;
        _collider.enabled = false;
        _target = target;
        transform.transform.DOScale(Vector3.zero, _scaleDuration).From(Vector3.one).SetEase(Ease.InBack);
    }
}
