using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    private IInteractable interactable;
    private bool triggerIsActive;

    private void Awake()
    {
        interactable = GetComponentInParent<IInteractable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            triggerIsActive = true;
            interactable.OnEnter(player);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!triggerIsActive)
            return;

        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            interactable.Interact(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            triggerIsActive = false;
            interactable.OnExit(player);
        }
    }
}
