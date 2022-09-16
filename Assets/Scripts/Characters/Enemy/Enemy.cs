using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Zenject;

public class Enemy : MonoBehaviour
{
    [SerializeField, Range(1, 10f)] private float _rotationSpeed = 5f;
    [SerializeField] private Rigidbody _rigidbody;

    private EnemyFactory _originFactory;
    private List<Vector3> _points;

    private int _currentCell = 0;
    private float _damage = 0;
    private int _nextCell = 0;
    private float _pathOffset;
    private float _health = 0f;
    private float _speed = 0f;
    private float _progress = 0f;

    [Inject] private Headquarters _headquarters;

    private Animator _animator;
    private float timeToDestroy = 0.75f;

    private HPBar _hpBar;

    public float Health => _health;
    public Rigidbody Rigidbody => _rigidbody;
    public EnemyFactory OriginFactory
    {
        get => _originFactory;
        set
        {
            _originFactory = value;
        }
    }

    private bool _initialized = false;

    public bool isDead { get; private set; }

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        _animator = GetComponent<Animator>();
        _animator.SetBool("DieBool", false);
        _hpBar = GetComponentInChildren<HPBar>();
    }

    private void Update()
    {
        if (!_initialized || _headquarters.IsDead)
            return;

        Move();
        Rotate();
    }

    private void Rotate()
    {
        if (isDead)
            return;

        Vector3 offset = transform.right * _pathOffset;
        Vector3 relativePos = (_points[_nextCell] + offset) - transform.position;

        Quaternion targetRot = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
    }

    [SerializeField] float dashSpeed = 1f;
    [SerializeField] float dashRotationSpeed = 0.25f;
    private void Move()
    {
        if (isDead)
            return;

        _progress += _speed * Time.deltaTime;

        while (_progress >= 1f)
        {
            _currentCell = _nextCell;
            _nextCell++;

            if (_nextCell >= (_points.Count - 1))
            {
                _animator.SetBool("GiveDamage", true);
                _headquarters.ApplyDamage(_damage);

                Quaternion targetRot = Quaternion.LookRotation(_headquarters.targetPoint.position - transform.position, Vector3.up);
                transform.DORotateQuaternion(targetRot, dashRotationSpeed);
                transform.DOMove(_headquarters.targetPoint.position, dashSpeed);

                Death();
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

    public void ApplyDamage(float damage)
    {
        _health -= damage;
        if (_hpBar != null)
            _hpBar.ChangeValue(_health);
        _animator.SetTrigger("TakeDamage");
        
        if (Health <= 0f)
        {
            Death(); 
        }
    }

    private void Death()
    {
        _animator.SetBool("DieBool", true);
        isDead = true;

        if (_hpBar != null)
            _hpBar.DisableBar();

        Invoke(nameof(Recycle), 2f);
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

