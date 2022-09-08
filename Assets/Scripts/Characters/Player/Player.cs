using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    private PlayerInventory _inventory;
    private PlayerMovement _movement;
    private PlayerAnimations _animation;
    private Headquarters _headquarters;

    public PlayerInventory Inventory => _inventory;
    public PlayerMovement Movement => _movement;
    public PlayerAnimations Animation => _animation;
    public Headquarters Headquarters => _headquarters;

    private void Awake()
    {
        _inventory = GetComponent<PlayerInventory>();
        _movement = GetComponent<PlayerMovement>();
        _animation = GetComponent<PlayerAnimations>();
    }

    public void SetHeadquarters(Headquarters headquarters)
    {
        _headquarters = headquarters;
    }
}
