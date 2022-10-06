using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

[SelectionBase]
public class Headquarters : MonoBehaviour
{
    [SerializeField, DisableInPlayMode, Range(1, 1000)] private float _health;
    [Label("Data Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]

    [Label("View Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private GameObject _headquartersBody;

    [Label("Points Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] protected Transform _dronePoint;
    [SerializeField] protected Transform _finishPoint;

    [Label("Animation Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private DamageAnimationSettings _damageAnimation;

    [Inject] private Game _game;
    private HPBar _hpBar;

    public bool IsDead => _health <= 0;
    public Transform DronePoint => _dronePoint;
    public Transform FinishPoint => _finishPoint;

    public static event Action OnDeath;

    private void Awake()
    {
        _game.SetHeadquarters(this);
        _hpBar = GetComponentInChildren<HPBar>();

        if (_hpBar != null)
            _hpBar.InitializationBar(_health);
    }
    public void ApplyDamage(float damage)
    {
        _health -= damage;

        if (_hpBar != null)
            _hpBar.ChangeValue(_health);

        if (_headquartersBody != null)
            _headquartersBody.transform.DOShakeScale(_damageAnimation.Duration, _damageAnimation.Strenght, _damageAnimation.Vibrato, _damageAnimation.Random);

        if (_health <= 0)
        {
            if (_hpBar != null)
                _hpBar.DisableBar();

            OnDeath?.Invoke();

            this.enabled = false;
        }
    }
}
