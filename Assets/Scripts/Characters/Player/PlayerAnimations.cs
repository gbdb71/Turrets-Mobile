using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _movement;

    public float AnimationSpeed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (_movement != null)
        {
            _animator.SetFloat(nameof(_movement.MoveVelocity), _movement.MoveVelocity);

            float targetLayer = _animator.GetLayerWeight(1);
            if (_movement.LayerWeight != targetLayer)
            {
                DOTween.To(() => targetLayer, x => targetLayer = x, _movement.LayerWeight, Random.Range(0.25f, 0.375f)).OnUpdate(() =>
                {
                    _animator.SetLayerWeight(1, targetLayer);
                });
            }
        }
    }
}
