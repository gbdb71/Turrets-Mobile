using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ToolBox.Pools;
using Zenject;
using System.Collections;
using Dreamteck.Splines;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(SplineFollower))]
public class Enemy : MonoBehaviour
{
    [Label("Movement Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private float _dashSpeed = 1f;
    [SerializeField] private float _dashRotationSpeed = 0.25f;
    [SerializeField] private Vector3 _finishOffset = Vector3.zero;

    [Label("Damage Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private float _flickerDuration = 0.25f;
    [SerializeField] private GameObject _damageParticle;
    [SerializeField, Range(0, 5f)] private float _damageTextOffset;

    [Label("Deceleration Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.1f, 1.5f)] private float _decelerationDrop = 2f;
    [SerializeField] private Texture lightTexture;

    [Label("Procedural animations", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private bool _spawnScaleAnimation = false;
    [SerializeField, EnableIf(nameof(_spawnScaleAnimation), true, Comparison = UnityComparisonMethod.Equal)] private float _scaleAnaimtionDuration = .5f;

    private PopupText _popupPrefab;
    private EnemyFactory _originFactory;
    private List<Vector3> _points;
    private Animator _animator;
    private HPBar _hpBar;
    [Inject] private Headquarters _headquarters;
    [Inject] private DiContainer _container;
    private Collider _collider;
    private Renderer _bodyRenderer;
    private SplineFollower _follower;

    private float _scale;
    private float _speed = 0;
    private float _damage = 0;
    private float _health = 0f;
    private RewardSettings _rewardSettings;

    private bool _initialized = false;
    private bool _isFinished = false;
    private float _deceleration = 0;

    private Vector3 _lastPosition;

    private Texture tempTexture;

    public float Health => _health;
    public bool IsDead { get; private set; }
    public Vector3 Velocity { get; private set; }
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
        _popupPrefab = Resources.Load<PopupText>("GameData/Effects/PopupText");

        _collider = GetComponent<Collider>();
        _follower = GetComponent<SplineFollower>();
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

        Velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;
        _follower.followSpeed = (_speed - (_speed * _deceleration));

        if (!_isFinished && _follower.GetPercent() >= .95f)
        {
            Finish();
        }
    }
    private void Start()
    {
        if (_spawnScaleAnimation)
        {
            transform.DOScale(_scale, _scaleAnaimtionDuration).From(Vector3.zero).SetEase(Ease.Linear);
        }
    }

    public void Initialize(float speed, float scale, float health, float damage, RewardSettings rewardSettings)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);

        _speed = speed;
        _scale = scale;
        _health = health;
        _damage = damage;
        _rewardSettings = rewardSettings;

        if (_hpBar != null)
            _hpBar.InitializationBar(health);

        _initialized = true;
    }
    public void SpawnOn(SplineComputer spline)
    {
        _follower.spline = spline;
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

    public void AddDeceleration(float value)
    {
        _deceleration += value;
        _deceleration = Mathf.Clamp(_deceleration, 0f, .8f);
    }
    public void ApplyDamage(float damage)
    {
        if (damage < 0)
            damage = 0;

        _health -= damage;

        if (_hpBar != null)
            _hpBar.ChangeValue(_health);

        StartCoroutine(DisplayDamage(damage));

        if (Health <= 0f && !IsDead)
        {
            Death();
        }
    }
    private void Death()
    {
        _animator.SetBool("Die", true);
        _collider.enabled = false;
        _follower.enabled = false;

        IsDead = true;

        if (_hpBar != null)
            _hpBar.DisableBar();

        if (!_isFinished)
        {
            SpawnRewards();
        }
    }

    private void SpawnRewards()
    {
        int amount = _rewardSettings.GetAmount();

        float r = Random.Range(.5f, .8f);

        for (int i = 0; i < amount; i++)
        {
            GameObject reward = _container.InstantiatePrefab(_rewardSettings.CurrencyPrefab, transform.position, Quaternion.identity, null);

            float angle = Random.Range(0, Mathf.PI * 2);
            Vector3 targetPoint = transform.position + new Vector3(Mathf.Sin(angle * (i + 1)) * r, .5f, Mathf.Cos(angle * (i + 1)) * r);

            reward.transform.DOScale(Vector3.one, .6f).From(Vector3.zero).SetEase(Ease.Linear);
            reward.transform.DOJump(targetPoint, 1.5f, 1, .6f);
        }

        IAbillity abillity = _rewardSettings.GetAbillity();

        if (abillity != null)
        {
            _container.Inject(abillity);

            float angle = Random.Range(0, Mathf.PI * 2);

            Vector3 targetPoint = transform.position + new Vector3(Mathf.Sin(angle) * r, .5f, Mathf.Cos(angle) * r);
            Transform abillityTransform = abillity.GetTransform();

            abillityTransform.position = transform.position;
            abillityTransform.DOScale(Vector3.one * 1.6f, .6f).From(Vector3.zero).SetEase(Ease.Linear);
            abillityTransform.DOJump(targetPoint, 4f, 1, 1f);
        }
    }

    private IEnumerator DisplayDamage(float damage)
    {
        _bodyRenderer.material.mainTexture = lightTexture;

        yield return new WaitForSeconds(_flickerDuration);

        _bodyRenderer.material.mainTexture = tempTexture;

        _damageParticle.Reuse(transform.position, Quaternion.identity);

        Vector3 targetPos = transform.position;
        targetPos.y += _damageTextOffset * transform.localScale.y;

        PopupText popup = _popupPrefab.gameObject.Reuse<PopupText>(targetPos, Quaternion.identity);
        popup.SetColor(Color.white);
        popup.SetText(((int)damage).ToString());
    }

    public void Recycle()
    {
        _originFactory.Reclaim(this);
    }
}

