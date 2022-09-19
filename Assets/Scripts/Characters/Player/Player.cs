using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    private PlayerInventory _inventory;
    private PlayerMovement _movement;
    private PlayerAnimations _animation;

    public PlayerInventory Inventory => _inventory;
    public PlayerMovement Movement => _movement;
    public PlayerAnimations PlayerAnimations => _animation;

    private void Awake()
    {
        _inventory = GetComponent<PlayerInventory>();
        _movement = GetComponent<PlayerMovement>();
        _animation = GetComponent<PlayerAnimations>();
    }

}
