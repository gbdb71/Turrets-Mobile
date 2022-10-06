using System;
using UnityEngine;
using Zenject;
using DG.Tweening;


[SelectionBase]
public class Headquarters : MonoBehaviour
{
    [SerializeField, DisableInPlayMode] private Settings _settings;

    [Label("View Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private GameObject _headquartersBody;

    [Label("Points Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] protected Transform _dronePoint;
    [SerializeField] protected Transform _finishPoint;

    [Label("Animation Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private DamageAnimationSettings _damageAnimation;

    [Inject] private Game _game;
    private HPBar _hpBar;
    private float _health = 0;

    public float Health { get { return _health + _health.Percent(SummableAbillity.GetValue(SummableAbillity.Type.HeadquartersHealth)); } private set { _health = value; } }
    public bool IsDead => Health <= 0;
    public Transform DronePoint => _dronePoint;
    public Transform FinishPoint => _finishPoint;

    public static event Action OnDeath;

    private void Awake()
    {
        _game.SetHeadquarters(this);
        _hpBar = GetComponentInChildren<HPBar>();

        _health = _settings.Health;

        if (_hpBar != null)
            _hpBar.InitializationBar(Health);
    }
    public void ApplyDamage(float damage)
    {
        Health -= damage;

        if (_hpBar != null)
            _hpBar.ChangeValue(Health);

        if (_headquartersBody != null)
            _headquartersBody.transform.DOShakeScale(_damageAnimation.Duration, _damageAnimation.Strenght, _damageAnimation.Vibrato, _damageAnimation.Random);

        if (Health <= 0)
        {
            if (_hpBar != null)
                _hpBar.DisableBar();

            OnDeath?.Invoke();

            this.enabled = false;
        }
    }

    [System.Serializable]
    public class Settings
    {
        [SerializeField, Range(1, 1000)] public float Health = 400f;
    }
}
