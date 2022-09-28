using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Label("Movement Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 10f)] private float _rotationSpeed = 5f;
    [SerializeField] private float _dashSpeed = 1f;
    [SerializeField] private float _dashRotationSpeed = 0.25f;
    [SerializeField] private Vector3 _finishOffset = Vector3.zero;

    [Label("Damage Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private float _flickerDuration = 0.25f;

    [Label("Deceleration Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.1f, 1.5f)] private float _decelerationDrop = 2f;

    [SerializeField] private Texture lightTexture;
    
    private EnemyFactory _originFactory;
    private List<Vector3> _points;
    private Animator _animator;
    private HPBar _hpBar;
    [Inject] private Headquarters _headquarters;
    private Rigidbody _rigidbody;
    private Renderer _bodyRenderer;

    private float _damage = 0;
    private float _health = 0f;
    private float _speed = 0f;
    private bool _initialized = false;

    private float _timeToDestroy = 0.75f;

    private int _currentCell = 0;
    private int _nextCell = 0;
    private float _pathOffset;
    private float _progress = 0f;
    private float _deceleration = 0;

    public float Health => _health;
    public bool IsDead { get; private set; }
    public Rigidbody Rigidbody => _rigidbody;
    public EnemyFactory OriginFactory
    {
        get => _originFactory;
        set
        {
            _originFactory = value;
        }
    }

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        _animator = GetComponent<Animator>();
        _hpBar = GetComponentInChildren<HPBar>();
        _bodyRenderer = GetComponentInChildren<Renderer>();

        tempTexture = _bodyRenderer.material.mainTexture;
    }

    private void Update()
    {
        if (!_initialized || _headquarters.IsDead)
            return;

        if (_deceleration > 0)
        {
            _deceleration -= _decelerationDrop * Time.deltaTime;
        }

        Move();
        Rotate();
    }

    private void Rotate()
    {
        if (IsDead)
            return;

        Vector3 offset = transform.right * _pathOffset;
        Vector3 relativePos = (_points[_nextCell] + offset) - transform.position;

        Quaternion targetRot = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (IsDead)
            return;

        _progress += (_speed - (_speed * _deceleration)) * Time.deltaTime;

        while (_progress >= 1f)
        {
            _currentCell = _nextCell;
            _nextCell++;

            if (_nextCell >= (_points.Count - 1))
            {
                Finish();
            }

            _progress -= 1f;
        }

        Vector3 offset = transform.right * _pathOffset;

        Vector3 from = _points[_currentCell];
        from += offset;

        Vector3 to = _points[_nextCell];
        to += offset;

        transform.position =
            Vector3.LerpUnclamped(from, to, _progress);
    }

    private void Finish()
    {
        _animator.SetBool("Finish", true);
        _headquarters.ApplyDamage(_damage);

        Vector3 targetPoint = _headquarters.FinishPoint.position + _finishOffset;

        Vector3 diff = targetPoint - transform.position;
        diff.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);

        targetRot.x = 0;
        targetRot.z = 0;

        transform.DORotateQuaternion(targetRot, _dashRotationSpeed).OnComplete(() =>
        {
            transform.DOMove(targetPoint, _dashSpeed);
        });

        Death();
    }

    public void AddDeceleration(float value)
    {
        _deceleration += value;
        _deceleration = Mathf.Clamp(_deceleration, 0f, .8f);
    }

    public void ApplyDamage(float damage)
    {
        _health -= damage;
        if (_hpBar != null)
            _hpBar.ChangeValue(_health);

        TakeDamage();

        if (Health <= 0f)
        {
            Death();
        }
    }

    Coroutine tempCorutine;
    Texture tempTexture;

    private void TakeDamage()
    {
        _bodyRenderer.material.mainTexture = lightTexture;

        if (tempCorutine != null)
            StopCoroutine(tempCorutine);

        StartCoroutine(ClearFill(_flickerDuration));
    }

    private IEnumerator ClearFill(float time)
    {
        yield return new WaitForSeconds(time);
        _bodyRenderer.material.mainTexture = tempTexture;
    }

    private void Death()
    {
        _animator.SetBool("Die", true);
        IsDead = true;

        if (_hpBar != null)
            _hpBar.DisableBar();
    }

    public void Initialize(float scale, float speed, float pathOffset, float health, float damage)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);

        _speed = speed;
        _pathOffset = pathOffset;
        _health = health;
        _damage = damage;

        if (_hpBar != null)
            _hpBar.InitializationBar(health);

        _initialized = true;
    }

    public void SpawnOn(List<Vector3> points)
    {
        _currentCell = 0;
        _nextCell = 1;
        _progress = 0f;

        _points = points;

        Vector3 spawnPoint = points[0];
        transform.position = spawnPoint;
    }

    public void Recycle()
    {
        _originFactory.Reclaim(this);
    }
}

