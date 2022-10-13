using UnityEngine;
using DG.Tweening;

public class Diamond : MonoBehaviour
{
    public int Amount;
    [SerializeField] private CurrencyType _currencyType;
    [SerializeField] private float _rotationDuration = 1f;
    [SerializeField] private float _scaleDuration = .7f;

    private void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 360, 0), _rotationDuration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
    }

    public void Destroy()
    {
            transform.transform.DOScale(Vector3.zero, _scaleDuration).From(Vector3.one).SetEase(Ease.Linear);

            transform.DOLocalMove(new Vector3(0, 1, 0), _scaleDuration - 0.2f).OnComplete(() =>
            {
                Destroy(gameObject);
            });

            enabled = false;
    }
}
