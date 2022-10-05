using UnityEngine;

public class Ammunition : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();    
    }

    private void OnEnable()
    {
        boxCollider.enabled = true;
    }

    private void OnDisable()
    {
        boxCollider.enabled = false;
    }
}
