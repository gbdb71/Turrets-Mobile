using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ToolBox.Pools;
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
    [SerializeField] private GameObject _damageParticle;
    [SerializeField] private GameObject _damageText;
    [SerializeField] private Vector3 _damageTextOffset;

    [Label("Deceleration Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.1f, 1.5f)] private float _decelerationDrop = 2f;
    [SerializeField] private Texture lightTexture;

    private EnemyFactory _originFactory;
    private List<Vector3> _points;
    private Animator _animator;
    private HPBar _hpBar;
    [Inject] private Headquarters _headquarters;
    [Inject] private Data _data;
    [Inject] private DiContainer _container;
    private Rigidbody _rigidbody;
    private Renderer _bodyRenderer;

    private float _damage = 0;
    private float _health = 0f;
    private float _speed = 0f;
    private RewardSettings _rewardSettings;

    private bool _initialized = false;
    private bool _isFinished = false;
    private int _currentCell = 0;
    private int _nextCell = 0;
    private float _pathOffset;
    private float _progress = 0f;
    private float _deceleration = 0;

    private Texture tempTexture;

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
    public void AddDeceleration(float value)
    {
        _deceleration += value;
        _deceleration = Mathf.Clamp(_deceleration, 0f, .8f);
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

        _isFinished = true;

        Death();
    }

    public void ApplyDamage(float damage)
    {
        _health -= damage;

        if (_hpBar != null)
            _hpBar.ChangeValue(_health);

        StartCoroutine(DisplayDamage(damage));

        if (Health <= 0f && !IsDead)
        {
            Death();
        }
    }
    private IEnumerator DisplayDamage(float damage)
    {
        _bodyRenderer.material.mainTexture = lightTexture;

        yield return new WaitForSeconds(_flickerDuration);

        _bodyRenderer.material.mainTexture = tempTexture;

        _damageParticle.Reuse(transform.position, Quaternion.identity);
        _damageText.Reuse<DamageText>(transform.position + _damageTextOffset, Quaternion.identity).SetText(((int)damage).ToString());
    }

    private void Death()
    {
        _animator.SetBool("Die", true);
        IsDead = true;

        if (_hpBar != null)
            _hpBar.DisableBar();

        if (!_isFinished)
        {
            int amount = _rewardSettings.GetAmount();
            amount = (int)(amount + ((float)amount).Percent(SummableAbillity.GetValue(SummableAbillity.Type.Loot)));
            
            Debug.Log($"{name} Amount {amount}");

            float r = Random.Range(.5f, .8f);

            for (int i = 0; i < amount; i++)
            {
                GameObject reward = _container.InstantiatePrefab(_rewardSettings.RewardPrefab, transform.position, Quaternion.identity, null);

                float angle = Random.Range(0, Mathf.PI * 2);
                Vector3 targetPoint = transform.position + new Vector3(Mathf.Sin(angle * (i + 1)) * r, .5f, Mathf.Cos(angle * (i + 1)) * r);

                reward.transform.DOScale(Vector3.one, .6f).From(Vector3.zero).SetEase(Ease.Linear);
                reward.transform.DOJump(targetPoint, 1.5f, 1, .6f);
            }
        }
    }

    public void Initialize(float scale, float speed, float pathOffset, float health, float damage, RewardSettings rewardSettings)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);

        _speed = speed;
        _pathOffset = pathOffset;
        _health = health;
        _damage = damage;
        _rewardSettings = rewardSettings;

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

