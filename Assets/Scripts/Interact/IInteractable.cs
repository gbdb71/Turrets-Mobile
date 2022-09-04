using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    public void OnEnter(Player player);
    public void Interact(Player player);
    public void OnExit(Player player);
}
