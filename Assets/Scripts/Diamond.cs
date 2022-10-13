using UnityEngine;
using Zenject;
using DG.Tweening;

public class Diamond : MonoBehaviour
{
    [SerializeField] private int _amount;
    [SerializeField] private CurrencyType _currencyType;
    [SerializeField] private float _rotationDuration = 1f;
    [SerializeField] private float _scaleDuration = .7f;

    [SerializeField] private ParticleSystem particle;

    [Inject] private Data _data;

    private void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 360, 0), _rotationDuration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            particle.Play();

            transform.DOScale(Vector3.zero, _scaleDuration).From(Vector3.one).SetEase(Ease.Linear).OnComplete(() =>
            {
                particle.Stop();
                Destroy(gameObject);
            });

            enabled = false;

            _data.User.TryAddCurrency(_currencyType, _amount); 
        }
    }
}
