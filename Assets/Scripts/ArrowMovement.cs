using DG.Tweening;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    [SerializeField] private float _moveDuration;
    [SerializeField] private float _moveAmount = 1f;

    private void Start()
    {
        float amount = transform.localPosition.z + _moveAmount;
        transform.DOLocalMoveZ(amount, _moveDuration).SetLoops(-1, LoopType.Yoyo);
    }
}
