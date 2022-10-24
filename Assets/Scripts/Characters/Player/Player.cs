using UnityEngine;
using Zenject;
using System.Collections;
using System.Collections.Generic;

[SelectionBase]
public class Player : MonoBehaviour
{
    [SerializeField] private ParticleSystem _diamondParticle;

    private PlayerInventory _inventory;
    private PlayerMovement _movement;
    private PlayerAnimations _animation;

    [Inject] private Data _data;

    public PlayerInventory Inventory => _inventory;
    public PlayerMovement Movement => _movement;
    public PlayerAnimations PlayerAnimations => _animation;

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
            if (_diamondParticle != null)
                _diamondParticle.Play();

            _data.User.TryAddCurrency(CurrencyType.Construction, diamond.Amount);
            diamond.Activate(transform);
        }
    }
}
