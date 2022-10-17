using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BaseAbillity : MonoBehaviour
{
    public Collider Collider { get; private set; }

    protected virtual void Awake()
    {
        Collider = GetComponent<Collider>();
    }

    public virtual void Clear() => Destroy(gameObject);
    public abstract void Activate();
    public virtual bool CanActivate => true;
}
