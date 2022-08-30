using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInventory _inventory;
    private PlayerMovement _movement;
    private PlayerAnimations _animation;

    public PlayerInventory Inventory => _inventory;
    public PlayerMovement Movement => _movement;
    public PlayerAnimations Animation => _animation;

    private void Awake()
    {
        _inventory = GetComponent<PlayerInventory>();
        _movement = GetComponent<PlayerMovement>();
        _animation = GetComponent<PlayerAnimations>();
    }

    private void OnTriggerEnter(Collider other)
    {
        

    }
}
