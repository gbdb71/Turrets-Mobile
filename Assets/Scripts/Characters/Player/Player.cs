using UnityEngine;
using Zenject;

[SelectionBase]
public class Player : MonoBehaviour
{
    [SerializeField] private ParticleSystem _diamondParticle;

    private PlayerInventory _inventory;
    private PlayerMovement _movement;
    private PlayerAnimations _animation;

    [Inject] private Map _map;
    [Inject] private Data _data;

    public PlayerInventory Inventory => _inventory;
    public PlayerMovement Movement => _movement;
    public PlayerAnimations PlayerAnimations => _animation;
    public Map Map => _map;
    public Data Data => _data;

    private void Awake()
    {
        _inventory = GetComponent<PlayerInventory>();
        _movement = GetComponent<PlayerMovement>();
        _animation = GetComponent<PlayerAnimations>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Diamond diamond))
        {
            _data.User.TryAddCurrency(CurrencyType.Construction, diamond.Amount);

            if (_diamondParticle != null)
                _diamondParticle.Play();

            diamond.transform.SetParent(transform);
            diamond.Destroy();
        }
    }
}
