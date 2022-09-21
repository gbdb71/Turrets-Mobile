using UnityEngine;
using DG.Tweening;

public class InteractTrigger : MonoBehaviour
{
    [Label("Interact Visual", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Vector3 _interactScale = new Vector3(1.1f, 1f, 1.1f);

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

            transform.DOScale(_interactScale, .3f).SetEase(Ease.InBounce);
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


            transform.DOScale(Vector3.one, .3f).SetEase(Ease.InOutBack);
        }
    }
}
