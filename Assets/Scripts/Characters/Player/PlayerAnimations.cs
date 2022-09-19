using UnityEngine;
using DG.Tweening;
using UnityEditor.Animations;

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
        if(_movement != null)
        {
            _animator.SetFloat(nameof(_movement.MoveVelocity), _movement.MoveVelocity);
            //_animator.SetFloat("PlayerWalk", AnimationSpeed);
            
            //blendTree.children[0].timeScale = AnimationSpeed;
            float currentLayerWeight = _animator.GetLayerWeight(1);
            if (_movement.LayerWeight != currentLayerWeight)
                DOTween.To(() => currentLayerWeight, x => currentLayerWeight = x, _movement.LayerWeight, Random.Range(0.25f, 0.375f)).OnUpdate(() => { _animator.SetLayerWeight(1, currentLayerWeight); });
                
        }
    }
}
