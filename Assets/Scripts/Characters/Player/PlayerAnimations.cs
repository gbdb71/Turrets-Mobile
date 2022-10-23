using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    private const int _animationLayer = 1;

    private Animator _animator;
    private Player _player;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player.Movement != null)
        {
            _animator.SetFloat(nameof(_player.Movement.MoveVelocity), _player.Movement.MoveVelocity);

            float currentLayer = _animator.GetLayerWeight(_animationLayer);
            float targetLayer = _player.Inventory.HasTurret ? 1 : 0;

            if (targetLayer != currentLayer)
            {
                DOTween.To(() => currentLayer, x => _animator.SetLayerWeight(_animationLayer, x), targetLayer, 0.25f);
            }
        }
    }
}
